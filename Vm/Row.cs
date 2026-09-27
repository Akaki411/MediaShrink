using System.Windows.Media;

namespace MediaShrink.Vm;

// One line of the table: a group of files with its color, share and size.
public sealed class Row
{
    public Brush Color { get; set; }
    public Brush Tint { get; set; }
    public string Ext { get; set; }
    public string Kind { get; set; }
    public string Codec { get; set; }
    public double Pct { get; set; }
    public string PctText { get; set; }
    public string SizeText { get; set; }
    public int Count { get; set; }
}
