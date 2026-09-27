using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using MediaShrink.Models;
using MediaShrink.Tools;
using NReco.VideoConverter;

namespace MediaShrink.Scan;

// Runs "ffmpeg -i" on a file and reads streams, duration and bitrate from its log.
public sealed class FfProbe : IProbe
{
    private static readonly Regex Dur = new Regex(@"Duration:\s*(\d+):(\d+):(\d+(?:\.\d+)?)", RegexOptions.Compiled);
    private static readonly Regex Bit = new Regex(@"Duration:.*?bitrate:\s*(\d+)\s*kb/s", RegexOptions.Compiled);
    private static readonly Regex Str = new Regex(@"Stream #\d+:\d+.*?: (Video|Audio): ([A-Za-z0-9_\-]+)", RegexOptions.Compiled);
    private readonly IFfTool _tool;

    public FfProbe(IFfTool tool)
    {
        _tool = tool;
    }

    public bool Fill(MediaFile f)
    {
        List<string> lines = new List<string>();
        try
        {
            FFMpegConverter conv = _tool.Make();
            conv.ExecutionTimeout = TimeSpan.FromSeconds(60);
            conv.LogReceived += (s, e) =>
            {
                lock (lines) lines.Add(e.Data ?? "");
            };
            conv.Invoke("-hide_banner -i \"" + f.Path + "\"");
        }
        catch (FFMpegException)
        {
        }
        catch (Exception)
        {
            return false;
        }

        string video = null;
        string audio = null;
        lock (lines)
        {
            foreach (string line in lines)
            {
                Match d = Dur.Match(line);
                if (d.Success)
                {
                    f.Secs = int.Parse(d.Groups[1].Value) * 3600 + int.Parse(d.Groups[2].Value) * 60
                             + double.Parse(d.Groups[3].Value, CultureInfo.InvariantCulture);
                    Match b = Bit.Match(line);
                    if (b.Success) f.Kbps = int.Parse(b.Groups[1].Value);
                }

                Match s = Str.Match(line);
                if (!s.Success) continue;
                if (s.Groups[1].Value == "Video")
                {
                    if (video == null && !line.Contains("attached pic")) video = s.Groups[2].Value;
                }
                else if (audio == null)
                {
                    audio = s.Groups[2].Value;
                }
            }
        }

        bool wantAudio = f.Kind == MediaKind.Audio;
        if (!wantAudio && video != null)
        {
            f.Kind = MediaKind.Video;
            f.Codec = video;
            return true;
        }
        if (audio != null)
        {
            f.Kind = MediaKind.Audio;
            f.Codec = audio;
            return true;
        }
        return false;
    }
}
