using FartingUnicorn;

namespace WebApplication1.Controllers;

public class BlogPost
{
    public string? Title { get; set; }
    public bool? IsDraft { get; set; }
    public Option<string>? Category { get; set; }
}
