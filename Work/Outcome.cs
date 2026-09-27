namespace MediaShrink.Work;

// The final numbers of a conversion run.
public sealed class Outcome
{
    public int Ok { get; set; }
    public int Fail { get; set; }
    public long In { get; set; }
    public long Out { get; set; }
    public bool Stopped { get; set; }
}
