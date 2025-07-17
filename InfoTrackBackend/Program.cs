using Blocks;
using Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var box = new BrainBox();

//  Forge a new MapDraft (workflow definition)
app.MapPost("/forge/mapdraft", (MapDraft draft) =>
{
    if (box.DraftStore.ContainsKey(draft.RefTag))
        return Results.BadRequest($"MapDraft '{draft.RefTag}' already exists.");

    if (draft.Phases.Count(p => p.Starter) != 1)
        return Results.BadRequest("Exactly one 'Starter' phase is required.");

    if (draft.Phases.GroupBy(p => p.Key).Any(g => g.Count() > 1))
        return Results.BadRequest("Duplicate phase keys are not allowed.");

    if (draft.Switches.Any(s => !draft.Phases.Any(p => p.Key == s.Destination)))
        return Results.BadRequest("All destinations must refer to valid phases.");

    box.DraftStore[draft.RefTag] = draft;
    return Results.Ok($"MapDraft '{draft.RefTag}' forged.");
});

// Retrieve an existing draft
app.MapGet("/forge/mapdraft/{id}", (string id) =>
{
    if (!box.DraftStore.TryGetValue(id, out var draft))
        return Results.NotFound($"No MapDraft found with ID '{id}'.");

    return Results.Ok(draft);
});

// Kickstart a new LiveTrace (workflow instance)
app.MapPost("/playline/kickstart", (string draftId) =>
{
    if (!box.DraftStore.TryGetValue(draftId, out var draft))
        return Results.BadRequest("Unknown draft.");

    var start = draft.Phases.FirstOrDefault(p => p.Starter && p.Usable);
    if (start == null)
        return Results.BadRequest("No usable starter phase.");

    var runId = Guid.NewGuid().ToString();
    var live = new LiveTrace
    {
        RunId = runId,
        DraftTag = draftId,
        NowAt = start.Key
    };

    box.ActiveRuns[runId] = live;
    return Results.Ok(new { TraceId = runId });
});

// Hop to the next state via a move
app.MapPost("/playline/hop", (string traceId, string moveCode) =>
{
    if (!box.ActiveRuns.TryGetValue(traceId, out var trace))
        return Results.NotFound("Trace ID not found.");

    if (!box.DraftStore.TryGetValue(trace.DraftTag, out var draft))
        return Results.BadRequest("Associated draft missing.");

    var move = draft.Switches.FirstOrDefault(m => m.Code == moveCode);
    if (move == null || !move.Usable)
        return Results.BadRequest("Invalid or disabled move.");

    if (!move.Origins.Contains(trace.NowAt))
        return Results.BadRequest("Cannot hop from current phase with this move.");

    if (draft.Phases.FirstOrDefault(p => p.Key == trace.NowAt)?.Terminator == true)
        return Results.BadRequest("Reached terminal phase. No further moves.");

    trace.NowAt = move.Destination;
    trace.HistoryTrail.Add((moveCode, DateTime.UtcNow));

    return Results.Ok("Hop complete.");
});

// View current status and history of an instance
app.MapGet("/playline/snapshot/{id}", (string id) =>
{
    if (!box.ActiveRuns.TryGetValue(id, out var trace))
        return Results.NotFound("No such live trace.");

    return Results.Ok(trace);
});

app.Run();
