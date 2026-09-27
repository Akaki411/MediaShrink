using System.Collections.Generic;
using ImageMagick;
using MediaShrink.Models;

namespace MediaShrink.Targets;

// Photo formats for Magick.NET, from the smallest results to the biggest.
public sealed class PhotoTargets : ITargetSource
{
    public MediaKind Kind
    {
        get { return MediaKind.Photo; }
    }

    public IEnumerable<Target> All()
    {
        List<Target> list = new List<Target>();

        PhotoTarget webp = Lossy("webp", "WebP", ".webp", MagickFormat.WebP, 80, 0.72);
        webp.Anim = true;
        webp.Defs["method"] = "6";
        list.Add(webp);

        list.Add(Lossy("avif", "AVIF", ".avif", MagickFormat.Avif, 55, 0.5));
        list.Add(Lossy("jxl", "JPEG XL", ".jxl", MagickFormat.Jxl, 80, 0.55));
        list.Add(Lossy("heic", "HEIC", ".heic", MagickFormat.Heic, 60, 0.55));

        PhotoTarget jpg = Lossy("jpg", "JPEG", ".jpg", MagickFormat.Jpeg, 80, 1.0);
        jpg.Alpha = false;
        jpg.Prog = true;
        jpg.Defs["sampling-factor"] = "4:2:0";
        list.Add(jpg);

        list.Add(Lossy("jp2", "JPEG 2000", ".jp2", MagickFormat.Jp2, 60, 1.05));

        PhotoTarget webpLs = Lossless("webp-ls", "WebP (без потерь)", ".webp", MagickFormat.WebP, 2.1);
        webpLs.Anim = true;
        webpLs.Defs["lossless"] = "true";
        webpLs.Defs["method"] = "6";
        list.Add(webpLs);

        PhotoTarget jxlLs = Lossless("jxl-ls", "JPEG XL (без потерь)", ".jxl", MagickFormat.Jxl, 1.8);
        jxlLs.Q = 100;
        list.Add(jxlLs);

        PhotoTarget png = Lossless("png", "PNG", ".png", MagickFormat.Png, 3.2);
        png.Q = 95;
        list.Add(png);

        PhotoTarget tifLzw = Lossless("tif-lzw", "TIFF · LZW", ".tif", MagickFormat.Tiff, 5.0);
        tifLzw.Comp = CompressionMethod.LZW;
        list.Add(tifLzw);

        PhotoTarget tifZip = Lossless("tif-zip", "TIFF · ZIP", ".tif", MagickFormat.Tiff, 3.6);
        tifZip.Comp = CompressionMethod.Zip;
        list.Add(tifZip);

        PhotoTarget tifJpg = Lossy("tif-jpg", "TIFF · JPEG", ".tif", MagickFormat.Tiff, 80, 1.0);
        tifJpg.Comp = CompressionMethod.JPEG;
        tifJpg.Alpha = false;
        list.Add(tifJpg);

        PhotoTarget gif = Lossy("gif", "GIF", ".gif", MagickFormat.Gif, 100, 3.0);
        gif.Anim = true;
        list.Add(gif);

        PhotoTarget tga = Lossless("tga", "TGA", ".tga", MagickFormat.Tga, 11.0);
        tga.Comp = CompressionMethod.RLE;
        list.Add(tga);

        list.Add(Lossless("bmp", "BMP", ".bmp", MagickFormat.Bmp, 14.0));
        return list;
    }

    private static PhotoTarget Lossy(string id, string name, string ext, MagickFormat fmt, uint q, double eff)
    {
        return new PhotoTarget
        {
            Id = id, Kind = MediaKind.Photo, Name = name, Ext = ext, Fmt = fmt, Q = q, Eff = eff, Lossy = true
        };
    }

    private static PhotoTarget Lossless(string id, string name, string ext, MagickFormat fmt, double eff)
    {
        return new PhotoTarget
        {
            Id = id, Kind = MediaKind.Photo, Name = name, Ext = ext, Fmt = fmt, Eff = eff, Lossy = false,
            Codec = name.Contains("без потерь") ? "без потерь" : ""
        };
    }
}
