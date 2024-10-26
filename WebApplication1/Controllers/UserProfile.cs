using DotNetThoughts.FartingUnicorn;

using FartingUnicorn;

namespace WebApplication1.Controllers;

public partial class WeatherForecastController
{
    [CreateMapper]
    public class UserProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsSubscribed { get; set; }
        public string[] Courses { get; set; }
        public Option<Pet> Pet { get; set; }

        public bool? IsGay { get; set; }
        public Pet? FavoritePet { get; set; }
    }
}
