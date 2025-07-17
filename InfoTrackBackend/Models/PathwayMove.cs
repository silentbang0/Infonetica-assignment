namespace Models;

public class PathwayMove
{
    public string Code { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public bool Usable { get; set; } = true;
    public List<string> Origins { get; set; } = new();
    public string Destination { get; set; } = string.Empty;
}
