using System;

namespace MediaShrink.Work;

// A progress report: how far we are, how long is left and what file is in work.
public sealed class Step
{
    public double Done { get; set; }
    public TimeSpan? Left { get; set; }
    public string Now { get; set; }
}
