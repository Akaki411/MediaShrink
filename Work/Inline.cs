using System;

namespace MediaShrink.Work;

// IProgress that calls its action right on the caller's thread, without queueing.
public sealed class Inline<T> : IProgress<T>
{
    private readonly Action<T> _act;

    public Inline(Action<T> act)
    {
        _act = act;
    }

    public void Report(T value)
    {
        _act(value);
    }
}
