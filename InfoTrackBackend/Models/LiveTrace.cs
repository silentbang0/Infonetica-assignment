namespace Models;

public class LiveTrace
{
    public string RunId { get; set; } = string.Empty;
    public string DraftTag { get; set; } = string.Empty;
    public string NowAt { get; set; } = string.Empty;
    public List<(string MoveCode, DateTime When)> HistoryTrail { get; set; } = new();
}
