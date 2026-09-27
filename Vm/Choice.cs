using MediaShrink.Core;
using MediaShrink.Targets;

namespace MediaShrink.Vm;

// One item of a format drop-down: the format name and how much it changes the size.
public sealed class Choice : Bindable
{
    private string _delta = "";
    private bool _gain;

    public Choice(Target target)
    {
        Target = target;
    }

    public Target Target { get; }

    public string Name
    {
        get { return Target.Name; }
    }

    public string Delta
    {
        get { return _delta; }
        private set { Set(ref _delta, value); }
    }

    public bool Gain
    {
        get { return _gain; }
        private set { Set(ref _gain, value); }
    }

    public void Show(double delta)
    {
        Delta = "≈ " + Fmt.Delta(delta);
        Gain = delta < 0;
    }
}
