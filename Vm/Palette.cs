using System.Linq;
using System.Windows.Media;

namespace MediaShrink.Vm;

// Soft colors for groups. Hue -1 is the gray "Other" color.
public static class Palette
{
    private static readonly string[] Codes =
    {
        "#4F86F7", "#F5A524", "#22B07D", "#EF5B5B", "#8B6CF0", "#1FB6D0",
        "#E85D9E", "#93C43A", "#F2762E", "#5C6BE0", "#17A99A", "#B364D9"
    };

    private static readonly Brush[] Fills = Codes.Select(c => Make(c, 255)).ToArray();
    private static readonly Brush[] Softs = Codes.Select(c => Make(c, 70)).ToArray();
    private static readonly Brush Gray = Make("#B6BCC6", 255);
    private static readonly Brush GraySoft = Make("#B6BCC6", 70);

    public static Brush Fill(int hue)
    {
        return hue < 0 ? Gray : Fills[hue % Fills.Length];
    }

    public static Brush Tint(int hue)
    {
        return hue < 0 ? GraySoft : Softs[hue % Softs.Length];
    }

    private static Brush Make(string code, byte alpha)
    {
        Color c = (Color)ColorConverter.ConvertFromString(code);
        SolidColorBrush b = new SolidColorBrush(Color.FromArgb(alpha, c.R, c.G, c.B));
        b.Freeze();
        return b;
    }
}
