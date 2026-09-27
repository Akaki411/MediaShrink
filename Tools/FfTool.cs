using System;
using System.Diagnostics;
using System.IO;
using NReco.VideoConverter;

namespace MediaShrink.Tools;

// Creates NReco converters; unpacks the built-in ffmpeg once, or uses ffmpeg.exe placed next to the program.
public sealed class FfTool : IFfTool
{
    private static readonly object Gate = new object();
    private static bool _ready;
    private readonly string _dir;

    public FfTool()
    {
        string own = AppDomain.CurrentDomain.BaseDirectory;
        _dir = File.Exists(Path.Combine(own, "ffmpeg.exe"))
            ? own
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MediaShrink", "ff");
    }

    public FFMpegConverter Make()
    {
        FFMpegConverter conv = Fresh();
        Prepare(conv);
        return conv;
    }

    public string Exe()
    {
        Prepare(Fresh());
        return Path.Combine(_dir, "ffmpeg.exe");
    }

    private FFMpegConverter Fresh()
    {
        FFMpegConverter conv = new FFMpegConverter();
        conv.FFMpegToolPath = _dir;
        conv.FFMpegProcessPriority = ProcessPriorityClass.BelowNormal;
        return conv;
    }

    private void Prepare(FFMpegConverter conv)
    {
        lock (Gate)
        {
            if (_ready) return;
            Directory.CreateDirectory(_dir);
            if (!File.Exists(Path.Combine(_dir, "ffmpeg.exe"))) conv.ExtractFFmpeg();
            _ready = true;
        }
    }
}
