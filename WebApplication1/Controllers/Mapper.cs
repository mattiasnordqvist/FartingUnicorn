//using DotNetThoughts.Results;

//using FartingUnicorn;

//using System.Text.Json;

//namespace WebApplication1.Controllers;

//public partial class WeatherForecastController
//{
//    public class Mapper
//    {
//        public record RequiredPropertyMissingError(string propertyName) : ErrorBase($"{propertyName} is required");
//        public record RequiredValueMissingError(string propertyName) : ErrorBase($"{propertyName} must have a value");
//        public static Result<UserProfile> Map(JsonElement json)
//        {
//            Result<Unit> result = UnitResult.Ok;
//            var userProfile = new UserProfile();


//            // required property in all senses. It must be present in the JSON and must have a value (not null in json)
//            if (!json.TryGetProperty("name", out var nameProperty))
//            {
//                // This handles the case when the property is completely missing
//                result = result.Or(Result<Unit>.Error(new RequiredPropertyMissingError("name")));
//            }
//            else
//            {
//                if (nameProperty.ValueKind == JsonValueKind.Null)
//                {
//                    result = result.Or(Result<Unit>.Error(new RequiredValueMissingError("name")));
//                }
//                userProfile.Name = nameProperty.GetString();
//            }
//            userProfile.Age = json.GetProperty("age").GetInt32();
//            userProfile.IsSubscribed = json.GetProperty("isSubscribed").GetBoolean();
//            userProfile.Courses = json.GetProperty("courses").EnumerateArray().Select(course => course.GetString()).ToArray();

//            if (json.GetProperty("pet").ValueKind == JsonValueKind.Null)
//            {
//                userProfile.Pet = new None<Pet>();
//            }
//            else
//            {
//                userProfile.Pet = new Some<Pet>(new Pet
//                {
//                    Name = json.GetProperty("pet").GetProperty("name").GetString(),
//                    Type = json.GetProperty("pet").GetProperty("type").GetString()
//                });
//            }

//            if (json.TryGetProperty("isGay", out var isGay))
//            {
//                userProfile.IsGay = isGay.GetBoolean();
//            }

//            if (json.TryGetProperty("favoritePet", out var favoritePet))
//            {
//                userProfile.FavoritePet = new Pet
//                {
//                    Name = favoritePet.GetProperty("name").GetString(),
//                    Type = favoritePet.GetProperty("type").GetString()
//                };
//            }

//            return result.Map(() => userProfile);
//        }
//    }
//}
