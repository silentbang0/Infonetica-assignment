using Models;

namespace Blocks;

public class BrainBox
{
    public Dictionary<string, MapDraft> DraftStore { get; } = new();
    public Dictionary<string, LiveTrace> ActiveRuns { get; } = new();
}
