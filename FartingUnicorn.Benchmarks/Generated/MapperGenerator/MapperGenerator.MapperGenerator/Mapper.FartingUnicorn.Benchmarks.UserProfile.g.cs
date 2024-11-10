using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Benchmarks.UserProfile> MapToFartingUnicorn_Benchmarks_UserProfile(JsonElement jsonElement, string[] path = null)
    {
        if(path is null)
        {
            path = ["$"];
        }
        /*object*/
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return Result<FartingUnicorn.Benchmarks.UserProfile>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }
        }
        var obj = new FartingUnicorn.Benchmarks.UserProfile();

        List<IError> errors = new();
        var isNamePropertyDefined = jsonElement.TryGetProperty("Name", out var jsonNameProperty);
        if (isNamePropertyDefined)
        {
            // String, isOption = False
            if (jsonNameProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Name"]));
            }
            else if (jsonNameProperty.ValueKind == JsonValueKind.String)
            {
                obj.Name = jsonNameProperty.GetString();
            }
            else
            {
                errors.Add(new ValueHasWrongTypeError([.. path, "Name"], "String", jsonNameProperty.ValueKind.ToString()));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "Name"]));
        }
        var isAgePropertyDefined = jsonElement.TryGetProperty("Age", out var jsonAgeProperty);
        if (isAgePropertyDefined)
        {
            // Int32, isOption = False
            if (jsonAgeProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Age"]));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "Age"]));
        }
        var isIsSubscribedPropertyDefined = jsonElement.TryGetProperty("IsSubscribed", out var jsonIsSubscribedProperty);
        if (isIsSubscribedPropertyDefined)
        {
            // Boolean, isOption = False
            if (jsonIsSubscribedProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "IsSubscribed"]));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "IsSubscribed"]));
        }
        var isCoursesPropertyDefined = jsonElement.TryGetProperty("Courses", out var jsonCoursesProperty);
        if (isCoursesPropertyDefined)
        {
            // , isOption = False
            if (jsonCoursesProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Courses"]));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "Courses"]));
        }
        var isPetPropertyDefined = jsonElement.TryGetProperty("Pet", out var jsonPetProperty);
        if (isPetPropertyDefined)
        {
            // Pet, isOption = False
            if (jsonPetProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Pet"]));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "Pet"]));
        }
        var isIsGayPropertyDefined = jsonElement.TryGetProperty("IsGay", out var jsonIsGayProperty);
        if (isIsGayPropertyDefined)
        {
            // Nullable, isOption = False
            if (jsonIsGayProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "IsGay"]));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "IsGay"]));
        }
        var isFavoritePetPropertyDefined = jsonElement.TryGetProperty("FavoritePet", out var jsonFavoritePetProperty);
        if (isFavoritePetPropertyDefined)
        {
            // Pet, isOption = False
            if (jsonFavoritePetProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "FavoritePet"]));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "FavoritePet"]));
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Benchmarks.UserProfile>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Benchmarks.UserProfile>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
