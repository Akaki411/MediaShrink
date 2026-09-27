using System;
using System.Collections.Generic;
using System.Threading;
using ImageMagick;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Work;

// Converts photos with Magick.NET, one thread per image (many images run at once). Animated formats keep their frames.
public sealed class PhotoConverter : IConverter
{
    static PhotoConverter()
    {
        ResourceLimits.Thread = 1;
    }

    public MediaKind Kind
    {
        get { return MediaKind.Photo; }
    }

    public void Run(MediaFile f, Target t, string dst, IProgress<double> log, CancellationToken ct)
    {
        PhotoTarget pt = (PhotoTarget)t;
        ct.ThrowIfCancellationRequested();
        using (MagickImageCollection col = new MagickImageCollection(f.Path))
        {
            if (col.Count > 1 && pt.Anim)
            {
                col.Coalesce();
                foreach (IMagickImage<byte> frame in col) Setup(frame, pt);
                col.Write(dst, pt.Fmt);
            }
            else
            {
                IMagickImage<byte> img = col[0];
                Setup(img, pt);
                img.Write(dst, pt.Fmt);
            }
        }
        log.Report(1);
    }

    private static void Setup(IMagickImage<byte> img, PhotoTarget pt)
    {
        img.AutoOrient();
        if (!pt.Alpha && img.HasAlpha) img.ColorAlpha(MagickColors.White);
        img.Quality = pt.Q;
        if (pt.Prog) img.Settings.Interlace = Interlace.Plane;
        if (pt.Comp.HasValue) img.Settings.Compression = pt.Comp.Value;
        foreach (KeyValuePair<string, string> d in pt.Defs) img.Settings.SetDefine(pt.Fmt, d.Key, d.Value);
    }
}
