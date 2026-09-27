using System;
using System.Collections.Generic;
using MediaShrink.Models;

namespace MediaShrink.Scan;

// Lists every extension we treat as photo, audio or video.
public sealed class KindMap : IKindMap
{
    private static readonly HashSet<string> Photo = Make(
        ".jpg .jpeg .jpe .jfif .png .gif .bmp .dib .tif .tiff .webp .avif .heic .heif .jxl " +
        ".jp2 .j2k .tga .ico .psd .pcx");

    private static readonly HashSet<string> Audio = Make(
        ".mp3 .aac .m4a .ogg .oga .opus .flac .wav .wma .aif .aiff .aifc .ac3 .eac3 .mka .amr " +
        ".ape .wv .mp2 .mpc .spx .dts .caf .au .m4b .tta");

    private static readonly HashSet<string> Video = Make(
        ".mp4 .m4v .mkv .webm .avi .mov .wmv .asf .flv .f4v .mpg .mpeg .mpe .ts .m2ts .mts .3gp " +
        ".3g2 .ogv .vob .divx .rm .rmvb .mxf .dv .qt");

    public MediaKind Of(string ext)
    {
        if (Photo.Contains(ext)) return MediaKind.Photo;
        if (Audio.Contains(ext)) return MediaKind.Audio;
        if (Video.Contains(ext)) return MediaKind.Video;
        return MediaKind.Other;
    }

    private static HashSet<string> Make(string list)
    {
        return new HashSet<string>(list.Split(' '), StringComparer.OrdinalIgnoreCase);
    }
}
