using System.Windows.Media;

namespace MediaShrink.Vm;

// One piece of the pie chart.
public sealed class Slice
{
    public Brush Color { get; set; }
    public double Pct { get; set; }
}
