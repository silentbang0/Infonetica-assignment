namespace Models;

public class MapDraft
{
    public string RefTag { get; set; } = string.Empty;
    public List<PulseState> Phases { get; set; } = new();
    public List<PathwayMove> Switches { get; set; } = new();
}
