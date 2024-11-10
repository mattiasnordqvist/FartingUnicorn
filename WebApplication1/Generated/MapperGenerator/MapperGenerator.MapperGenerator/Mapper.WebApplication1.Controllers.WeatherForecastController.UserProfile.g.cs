using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<WebApplication1.Controllers.WeatherForecastController.UserProfile> MapToWebApplication1_Controllers_WeatherForecastController_UserProfile(JsonElement jsonElement, string[] path = null)
    {
        /*object*/
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return Result<WebApplication1.Controllers.WeatherForecastController.UserProfile>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }
        }
        var obj = new WebApplication1.Controllers.WeatherForecastController.UserProfile();

        List<IError> errors = new();
        var isNamePropertyDefined = jsonElement.TryGetProperty("Name", out var jsonNameProperty);
        if (isNamePropertyDefined)
        {
            // String, isOption = False
            if (jsonNameProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Name"]));
            }
            if (jsonNameProperty.ValueKind == JsonValueKind.String)
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
            obj.Name = null;
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
            obj.Age = null;
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
            obj.IsSubscribed = null;
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
            obj.Courses = null;
        }
        var isPetPropertyDefined = jsonElement.TryGetProperty("Pet", out var jsonPetProperty);
        if (isPetPropertyDefined)
        {
            // Pet, isOption = True
            if (jsonPetProperty.ValueKind == JsonValueKind.Null)
            {
                obj.Pet = new None<Pet>();
            }
        }
        else
        {
            obj.Pet = null;
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
            obj.IsGay = null;
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
            obj.FavoritePet = null;
        }
        if(errors.Any())
        {
            return Result<WebApplication1.Controllers.WeatherForecastController.UserProfile>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<WebApplication1.Controllers.WeatherForecastController.UserProfile>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
