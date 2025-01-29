namespace MARSIS.Actors;

public sealed record Tick
{
    public static readonly Tick Instance = new();
    private Tick() { }
}
