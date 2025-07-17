namespace Models;

public class PulseState
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool Starter { get; set; } = false;
    public bool Terminator { get; set; } = false;
    public bool Usable { get; set; } = true;
}
