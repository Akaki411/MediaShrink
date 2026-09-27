using System;

namespace MediaShrink.Core;

// Turns sizes, percents and time spans into short text for the screen.
public static class Fmt
{
    private static readonly string[] Units = { "Б", "КБ", "МБ", "ГБ", "ТБ" };

    public static string Size(long bytes)
    {
        double v = bytes;
        int i = 0;
        while (v >= 1024 && i < Units.Length - 1)
        {
            v /= 1024;
            i++;
        }
        return i == 0 ? bytes.ToString("N0") + " Б" : v.ToString("N1") + " " + Units[i];
    }

    public static string Pct(double p)
    {
        return p.ToString("0.0") + " %";
    }

    public static string Delta(double d)
    {
        long n = (long)Math.Round(Math.Abs(d) * 100);
        if (n == 0) return "0 %";
        return (d < 0 ? "−" : "+") + n + " %";
    }

    public static string Span(TimeSpan t)
    {
        if (t.TotalHours >= 1) return (int)t.TotalHours + " ч " + t.Minutes.ToString("00") + " мин";
        if (t.TotalMinutes >= 1) return t.Minutes + " мин " + t.Seconds.ToString("00") + " с";
        return t.Seconds + " с";
    }
}
