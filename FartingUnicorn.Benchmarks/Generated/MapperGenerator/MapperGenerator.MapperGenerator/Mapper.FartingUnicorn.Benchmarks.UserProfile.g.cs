using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Benchmarks.UserProfile> MapToFartingUnicorn_Benchmarks_UserProfile(JsonElement jsonElement, string[] path = null)
    {
        /*object*/
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return Result<FartingUnicorn.Benchmarks.UserProfile>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }
        }
        var obj = new FartingUnicorn.Benchmarks.UserProfile();

        Result<Unit> compositeResult = UnitResult.Ok;
        var isNamePropertyDefined = jsonElement.TryGetProperty("Name", out var jsonNameProperty);
        if (isNamePropertyDefined)
        {
            // String
            var mapResult = MapString(jsonNameProperty, /*mapperOptions,*/ [.. path, Name]);
            if (mapResult.Success)
            {
                obj.Name = mapResult.Value;
            }
            else
            {
                compositeResult = compositeResult.Or(mapResult);
            }
        }
        else
        {
        }
        var isAgePropertyDefined = jsonElement.TryGetProperty("Age", out var jsonAgeProperty);
        if (isAgePropertyDefined)
        {
            // Int32
            if (mapResult.Success)
            {
                obj.Age = mapResult.Value;
            }
            else
            {
                compositeResult = compositeResult.Or(mapResult);
            }
        }
        else
        {
        }
        var isIsSubscribedPropertyDefined = jsonElement.TryGetProperty("IsSubscribed", out var jsonIsSubscribedProperty);
        if (isIsSubscribedPropertyDefined)
        {
            // Boolean
            if (mapResult.Success)
            {
                obj.IsSubscribed = mapResult.Value;
            }
            else
            {
                compositeResult = compositeResult.Or(mapResult);
            }
        }
        else
        {
        }
        var isCoursesPropertyDefined = jsonElement.TryGetProperty("Courses", out var jsonCoursesProperty);
        if (isCoursesPropertyDefined)
        {
            // 
            if (mapResult.Success)
            {
                obj.Courses = mapResult.Value;
            }
            else
            {
                compositeResult = compositeResult.Or(mapResult);
            }
        }
        else
        {
        }
        var isPetPropertyDefined = jsonElement.TryGetProperty("Pet", out var jsonPetProperty);
        if (isPetPropertyDefined)
        {
            // Pet
            if (mapResult.Success)
            {
                obj.Pet = mapResult.Value;
            }
            else
            {
                compositeResult = compositeResult.Or(mapResult);
            }
        }
        else
        {
        }
        var isIsGayPropertyDefined = jsonElement.TryGetProperty("IsGay", out var jsonIsGayProperty);
        if (isIsGayPropertyDefined)
        {
            // Nullable
            if (mapResult.Success)
            {
                obj.IsGay = mapResult.Value;
            }
            else
            {
                compositeResult = compositeResult.Or(mapResult);
            }
        }
        else
        {
        }
        var isFavoritePetPropertyDefined = jsonElement.TryGetProperty("FavoritePet", out var jsonFavoritePetProperty);
        if (isFavoritePetPropertyDefined)
        {
            // Pet
            if (mapResult.Success)
            {
                obj.FavoritePet = mapResult.Value;
            }
            else
            {
                compositeResult = compositeResult.Or(mapResult);
            }
        }
        else
        {
        }
        if(!compositeResult.Success)
        {
            return Result<FartingUnicorn.Benchmarks.UserProfile>.Error(compositeResult.Errors);
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
