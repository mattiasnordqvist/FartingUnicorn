# Farting Unicorn

There's a lot of problems with asp.nets default deserialization process.

```csharp
[Post]
public async Task CreateBlogPost([FromBody] BlogPost blogPost) { ... }

public class BlogPost {
  public string Title { get; set; }
}
```
Sending an http request message to the endpoint above with the following json works fine.
```json
{
  "Title": "Farting Unicorns"
}
```

ASP.NET deserializes the json, creates a BlogPost instance and puts "Farting Unicorns" in the Title property. So far so good.

Supposing nullable reference types are enabled (they should be nowadays), this Title-property is considered required, since it is not nullable. Let's see what happens with some other json payloads

```json
{
  "Tightle": "Farting unibrows"
}
```
Title is misspelled. The `Title` field is not present in the Json. ASP.NET responds with a HTTP 400 with the following content.
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "The Title field is required."
    ]
  },
  "traceId": "00-f723cffb02dbfe1d113aa918334acaec-985b4a3cadc1b88d-00"
}
```
Ok! Looks good. 
How about this Json?
```json
{
  "Title": null
}
```
The title field is there, but it is explicitly set to null!
ASP.NET responds just the same:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "The Title field is required."
    ]
  },
  "traceId": "00-f723cffb02dbfe1d113aa918334acaec-985b4a3cadc1b88d-00"
}
```
This seems fine! It's behaving pretty good! We can get on with our blogpost. Let's say we want to indicate whether this blogpost is a draft or if it is published.

```csharp
public class BlogPost {
  public string Title { get; set; }
  public bool IsDraft { get; set; }
}
```
Looks like the `IsDraft`-property should behave like the `Title`-property. It is not nullable, so it should be required. We can test our happy cases and see that json like this works fine.
```json
{
  "Title": "Farting Unicorns",
  "IsDraft": true
}
```
```json
{
  "Title": "Farting Unicorns",
  "IsDraft": false
}
```
The json is deserialized, objects are created and their properties are populated with the expected values. Great!  
  
This is where things starts to break down for ASP.NET though. Its not pretty.  
Let's see what happens if we misspell or leave out `IsDraft`-property. 
```json
{
  "Title": "Farting Unicorns",
  "DisRaft": true
}
```
We would expect, that just like with the missing `Title`-property, ASP.NET responds with an error something like
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "The IsDraft field is required."
    ]
  },
  "traceId": "00-f723cffb02dbfe1d113aa918334acaec-985b4a3cadc1b88d-00"
}
```
This is not what happens though. ASP.NET accepts our perfectly valid json, creates a BlogPost instance with Title = "Farting Unicorns" and IsDraft = false. What? Where did `false` come from?  
The details on why this happens, is related to booleans being value-types, while strings are reference types, in conjunction with how nullable reference types works. Let's not delve deep into this rn. What can we do to fix it?  
Well, there's a `[Required]`-attribute. Let's apply it!
```csharp
 public class BlogPost
 {
     public string Title { get; set; }

     [Required]
     public bool IsDraft { get; set; }
 }
 ```
 Let's try this json again
 ```json
{
  "Title": "Farting Unicorns",
  "DisRaft": true
}
```
 Still no error messages! `IsDraft`-property is still populated with `false` and ASP.NET happily continues without warning us of the missing field.  
 Here's the deal.  
 > The RequiredAttribute attribute specifies that when a field on a form is validated, the field must contain a value. A validation exception is raised if the property is null, contains an empty string (""), or contains only white-space characters.  
 
 I don't know what "form" microsoft is talking about, but it seems like the `Required`-attribute only will generate an error if the field is null. This indicates that this attribute has nothing to do with actual deserialization-process. It will be validated against _after_ the Blogpost instance has already been constructed. With `IsDraft`being a value-type, it can't be set to null, but instead gets its `default` value when the object is created. The `default` value for a boolean is `false`. (Read https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/default-values.) The field is indeed not null, and the validation passes. The solution? Mark `IsDraft` as `nullable`! 
 ```csharp
  public class BlogPost
 {
     public string Title { get; set; }

     [Required]
     public bool? IsDraft { get; set; }
 }
 ```
 With this small change, `IsDraft` is no longer just a boolean. It is now a struct (still not a reference type) with very specific behavior. (Read https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-nullable%7Bt%7D). Without understanding exactly how this works, we can observe the effects.
  ```json
{
  "Title": "Farting Unicorns",
  "DisRaft": true
}
```

The json above finally gives us an error message:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "IsDraft": [
      "The IsDraft field is required."
    ]
  },
  "traceId": "00-e284ecf34d1faa8e95ad73939b85fb00-837349e397fc070f-00"
}
```

This actually works pretty well with multiple errors too! Say both fields are missing. The following json
```json 
{}
```
produces a response of
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "The Title field is required."
    ],
    "IsDraft": [
      "The IsDraft field is required."
    ]
  },
  "traceId": "00-3ac8f6d0f3e24dff8ebe8b3d0bd2c095-1b1ed8362bf2268d-00"
}
```
✅ Required fields 
✅ Multiple error messages

Let's go! 

Oh by the way. What would happen if we pass this json to the endpoint?

```json
{
  "Title": 123456,
  "IsDraft": "DRAFT"
}
```
This is perfectly valid json, but the data types does not match what we expect.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "blogPost": [
      "The blogPost field is required."
    ],
    "$.Title": [
      "The JSON value could not be converted to System.String. Path: $.Title | LineNumber: 1 | BytePositionInLine: 17."
    ]
  },
  "traceId": "00-58bdf457240790c35b1bc74779d8e926-1d39473e24194020-00"
}
```
Ok...  
What is the `blogPost`-field? What does `$.Title` mean? What tf does `System.String. Path: $.Title | LineNumber: 1 | BytePositionInLine: 17.` mean? And... where is the error message regarding `IsDraft`, which is too wrongly typed?
These messages are not at all as good as the ones we previously got.  We're back at square one.

This happens because ASP.NET is conflating 2 very different concepts. `Deserializing JSON` and `Mapping values to an object instance`. Actually, this is not a problem originating in ASP.NET. It is how the `System.Text.Json` serializer works.

This code would actually result in the same behaviour.
```csharp
var blogPost = JsonSerializer.Deserialize<BlogPost>("""
    {
      "Title": 123456,
      "IsDraft": "DRAFT"
    }
    """);
```
This throws an exception which we recognize from above.
> System.Text.Json.JsonException: 'The JSON value could not be converted to System.String. Path: $.Title | LineNumber: 1 | BytePositionInLine: 17.'

What can we do if we want our readable error messages back? Actually I can't find any agreed upon solution to this (plz tell me if there is!). A solution would be to decouple the deserialization from the object creation. `System.Text.Json` can deserialize onto a `JsonElement` object. Any valid json will pass, and we can then write our own code to map from `JsonElement` to `BlogPost`. I'm not sure I want to go down this route just yet, so let's put this problem aside for the moment, and investigate another interesting situation.

RESTful PUTs and PATCHes.  
It is generally agreed upon that a PUT should replace the reource at the URL with the provided resource in it's entirety. This means, if we want to update the BlogPosts title, we must PUT a complete json representation of the object with a new Title while making sure we don't change the value of IsDraft. I've found this problematic. (not related: ~~At the server side, I must inspect the received object, and figure out the clients intent. If IsDraft has changed from false to true, I want to trigger side-effects. There can also be validation to consider. Maybe you're not allowed to unpublish a published post, or change the title of a published blog post. To summarize, three things must happen: 1. Validate the requested state. 2. Validate that there is a valid transition from current state to the requested state. 3. Possibly trigger side-effects based on the changes made.~~)

There's a solution for this. The PATCH method. It is supposed to allow partial updates of an entity. There are different ways to implement PATCH. https://www.rfc-editor.org/rfc/rfc6902 proposes a rigourous "operation"-describing json structure, which honestly feels more RPC than REST. https://www.rfc-editor.org/rfc/rfc7396 on the other hand, describes a "merge" patch. I like this one! If a field is left out, it means we don't want to change it. If you want to set a field to null explicitly, you do so by including the field with a value of `null`! This looks promising! There's a semantic difference between a missing field and a field with the value null! 

However, when `System.Test.Json` translates from json to an object, it can't really represent the "subtle" difference between a missing field and a "nulled-out" field. The clr-related property will always exist, and its value will be `null` in both of these cases. As a result, we can't infer the clients intent. Did they leave out the field, or did they want to explicitly set the value to `null`?

This is the second roadblock related to `JsonSerializer.Deserialize<T>`. I think we must bring our own deserialization process if we want to fix this and the earlier issue we found!  

What are our requirements?
1. Only invalid JSON should throw serialization exceptions.
2. Being able to differentiate between left out, and explicitly nulled fields.
3. Return errors describing all invalid values, not just the first we hit.
4. Return a strongly typed object!

How can we in C# represent a missing field in a strongly typed object? I think the best way is to let `null` denote a missing field. This means that for a PATCH request we must make all optional field nullable.

```
public class BlogPost
{
    public string? Title { get; set; }
    public bool? IsDraft { get; set; }
    public string? Category { get;set; } 
}
```
So how do we differentiate between a missing field, and an explicitly nulled-out field? Null would make sense here too, right? However, in C#, we can't have nullable nullable fields. :(
```csharp
public class BlogPost
{
    public string? Title { get; set; }
    public bool? IsDraft { get; set; }
    public string?? Category { get;set; } 
}
```
![image](https://github.com/user-attachments/assets/becfb62f-9632-4f53-bea6-8a6b95be7b39)


Maybe we can create an option type like this?  
![image](https://github.com/user-attachments/assets/4f9ca35b-f5cf-4498-ac43-8a2acc790c92)


```csharp
public class BlogPost
{
    public string? Title { get; set; }
    public bool? IsDraft { get; set; }
    public Option<string>? Category { get; set; }
}
```

Semantics in a PATCH request:
`Title` is a non-nullable field. `?` makes it possible to leave it out completely.
`IsDraft`is a non-nullable field. `?` makes it possible to leave it out completely.
`Category` is nullable. `?` makes it possible to leave it out completely. None/Some used to declare the value of the field. 

| Json     | C#      |
| -------- | ------- |
| missing  | null    |
| null     | None    |
| 5        | Some(5) |

In a PUT request, the C# type would look like this instead: 
 ```csharp
public class BlogPost
{
    public string Title { get; set; }
    public bool IsDraft { get; set; }
    public Option<string> Category { get; set; }
}
```
All fields must exist on the object, but category can be set to "None".

I've come to the conclusion that the default deserialize-to-type of System.Text.Json ain't good for simple Merge-PATCHES, or even anyone who wants to distinguish a missing field from a nulled field. 

My approach to solve this would first involve separating the two different concerns. Deserializing to JsonElement works perfectly with System.Text.Json so let's keep doing that. Then, all that is needed is to map a JsonElement to a Type. Like this:

```csharp
  var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
    {
      "Title": "Farting Unicorns",
      "IsDraft": true
    }
    """);
  var blogPost = Mapper.Map<BlogPost>(jsonElement);
```

I realize that this is probably going to be less performant and more memory-intensive. We'll see later on.

So we've decided the following:
- `T`
  - A missing field will result in an error
  - A json null value will result in an error
  - A json value will be represented as its corresponding clr type (bool, int, string, object, array).
- `Nullable<T>`
  - A missing json-field will be represented with a clr `null` value.
  - A json null value will result in an error
  - A json value will be represented as its corresponding clr type (bool, int, string, object, array).
- `Option<T>`
  - A missing json-field will result in an error
  - A json null value will be represented with a `None<T>`
  - A json value will be represented as `Some<T>`
- `Nullable<Option<T>>`
    - A missing field will be represented with a clr `null` value
    - A json null value will be represented with a `None<T>`
    - A json value will be represented with a `Some<T>`
  
- ✅ The `Option`-type
- ✅ Create `JsonElement to Type`-Mapper
- ✅ Custom converters for Mapper
  - ✅ Verify mapper works well with Enums (included a CustomConverter for EnumsAsStrings
- ⭕ Add support for records
- ⭕ Better documentation and validation/exceptions on usage of invalid types. Like, are we expecting an empty constructor? Do we support `Option<Nullable<T>>`?
- ⭕ Case insensitivity please
- ⭕ Rewrite Mapper as SourceGenerator
- ✅ Write Source Generator for Minimal Apis BindAsync
  - ⭕ Make MapperOptions somehow available to generated code. 
- ❌ Support Microsofts OpenApi-implementation. (seems impossible right now)
- ⭕ Add Swagger support
- ⭕ Create IInputFormatter for ASP.NET


# How to use

Install FartingUnicorn-package.  
Create your input DTOs in the way we talked about above. Currently, only Classes with an empty constructor are supported. Make fields that are not required in json nullable. Use the `Option<T>` type for fields that should be nullable.  

Now you can parse the json and map it to your DTO like this:
```csharp
using var json = await JsonDocument.ParseAsync(jsonAsText);
var rootElement = json.RootElement;
var mapperResult = Mapper.Map<YourDto>(rootElement);
if(mapperResult.Success)
  // mapperResult.Value will contain your parsed DTO.
```

## Custom Converters
Mapper.Map can take a MapperOptions as parameter. Custom converters can be added through the MapperOptions.  
The library comes with a built-in converter you can use if you want your DTOs to contain `Enum`s that maps to json strings.

```csharp
 public class BlogPost
 {
     public string Title { get; set; }
     public BlogPostStatus Status { get; set; }
 }

 public enum BlogPostStatus { Draft, Published }
```
```csharp
 var _mapperOptions = new MapperOptions();
 _mapperOptions.AddConverter(new EnumAsStringConverter());
 var json = JsonSerializer.Deserialize<JsonElement>("""
   {"Title":"Farting Unicorns","Status":"Draft"}
   """);
 var blogPost = Mapper.Map<BlogPost>(json, mapperOptions: _mapperOptions);
```

## Minimal API
If you want to take the dto as a parameter in a minimal api endpoint, there is a source generator available to generate a BindAsync-method that deserializes the json and creates a DTO for you.

Install FartingUnicorn.MinimalApi

Make your DTO partial and add a `GenerateBindAsyncAttribute` attribute to your DTO.

```csharp
using DotNetThoughts.FartingUnicorn.MinimalApi;

[GenerateBindAsync]
public partial class UserProfile
{
    public string Name { get; set; }
    public int Age { get; set; }
    public bool IsSubscribed { get; set; }
    public string[] Courses { get; set; }
    public Option<Pet> Pet { get; set; }
}
```

This binder will: 
1. Create a bad request response if content type ain't application/json
2. throw a serializationexception if the request contains invalid json
3. throw an ValueOrThrowException if there are any validation issues.

You can handle these exception using the strange UseExceptionHandler-stuff. Here's an example

```csharp
 app.UseExceptionHandler(app =>
 {
     app.Run(async c =>
     {
         var exceptionHandlerPathFeature =
             c.Features.Get<IExceptionHandlerPathFeature>();

         if (exceptionHandlerPathFeature?.Error is ValueOrThrowException e)
         {
             c.Response.StatusCode = StatusCodes.Status400BadRequest;
             c.Response.ContentType = "application/json";
             await c.Response.WriteAsJsonAsync(new
             {
                 Success = false,
                 Errors = e.Errors.Select(x => new { x.Type, x.Message, Data = x.GetData() })
             });
         }
     });
 });
```
