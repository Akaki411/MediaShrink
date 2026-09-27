using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MediaShrink.Vm;

namespace MediaShrink.Views;

// Donut chart that draws slices by percent and prints a total and a caption in the middle.
public sealed class PieChart : FrameworkElement
{
    public static readonly DependencyProperty SlicesProperty = Reg<List<Slice>>("Slices");
    public static readonly DependencyProperty TotalProperty = Reg<string>("Total");
    public static readonly DependencyProperty CaptionProperty = Reg<string>("Caption");

    private static readonly Brush Track = Solid(0xEE, 0xF0, 0xF4);
    private static readonly Brush Ink = Solid(0x1F, 0x29, 0x37);
    private static readonly Brush Muted = Solid(0x6B, 0x72, 0x80);
    private static readonly Pen Gap = MakePen();

    public List<Slice> Slices
    {
        get { return (List<Slice>)GetValue(SlicesProperty); }
        set { SetValue(SlicesProperty, value); }
    }

    public string Total
    {
        get { return (string)GetValue(TotalProperty); }
        set { SetValue(TotalProperty, value); }
    }

    public string Caption
    {
        get { return (string)GetValue(CaptionProperty); }
        set { SetValue(CaptionProperty, value); }
    }

    protected override void OnRender(DrawingContext dc)
    {
        double outer = Math.Min(ActualWidth, ActualHeight) / 2 - 4;
        if (outer < 20) return;
        double inner = outer * 0.66;
        Point mid = new Point(ActualWidth / 2, ActualHeight / 2);
        double ring = outer - inner;

        dc.DrawEllipse(null, new Pen(Track, ring), mid, (outer + inner) / 2, (outer + inner) / 2);
        double angle = -90;
        foreach (Slice s in Slices ?? new List<Slice>())
        {
            double sweep = s.Pct * 3.6;
            if (sweep >= 359.9)
            {
                dc.DrawEllipse(null, new Pen(s.Color, ring), mid, (outer + inner) / 2, (outer + inner) / 2);
            }
            else if (sweep > 0.3)
            {
                dc.DrawGeometry(s.Color, Gap, Arc(mid, outer, inner, angle, angle + sweep));
            }
            angle += sweep;
        }
        Text(dc, Total, 22, true, Ink, mid.Y - 20);
        Text(dc, Caption, 12, false, Muted, mid.Y + 8);
    }

    private void Text(DrawingContext dc, string text, double size, bool bold, Brush brush, double top)
    {
        if (string.IsNullOrEmpty(text)) return;
        Typeface face = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal,
            bold ? FontWeights.SemiBold : FontWeights.Normal, FontStretches.Normal);
        FormattedText ft = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face,
            size, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        dc.DrawText(ft, new Point((ActualWidth - ft.Width) / 2, top));
    }

    private static Geometry Arc(Point mid, double outer, double inner, double from, double to)
    {
        bool big = to - from > 180;
        StreamGeometry geo = new StreamGeometry();
        using (StreamGeometryContext ctx = geo.Open())
        {
            ctx.BeginFigure(At(mid, outer, from), true, true);
            ctx.ArcTo(At(mid, outer, to), new Size(outer, outer), 0, big, SweepDirection.Clockwise, true, false);
            ctx.LineTo(At(mid, inner, to), true, false);
            ctx.ArcTo(At(mid, inner, from), new Size(inner, inner), 0, big, SweepDirection.Counterclockwise, true, false);
        }
        geo.Freeze();
        return geo;
    }

    private static Point At(Point mid, double r, double deg)
    {
        double rad = deg * Math.PI / 180;
        return new Point(mid.X + r * Math.Cos(rad), mid.Y + r * Math.Sin(rad));
    }

    private static DependencyProperty Reg<T>(string name)
    {
        return DependencyProperty.Register(name, typeof(T), typeof(PieChart),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.AffectsRender));
    }

    private static Brush Solid(byte r, byte g, byte b)
    {
        SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        return brush;
    }

    private static Pen MakePen()
    {
        Pen pen = new Pen(Brushes.White, 2);
        pen.LineJoin = PenLineJoin.Round;
        pen.Freeze();
        return pen;
    }
}
