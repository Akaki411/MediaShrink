using System;
using System.IO;
using System.Threading;
using MediaShrink.Estimate;
using MediaShrink.Models;
using MediaShrink.Targets;
using MediaShrink.Tools;
using NReco.VideoConverter;

namespace MediaShrink.Work;

// Converts audio or video through NReco (ffmpeg). Tries the GPU first and repeats on the CPU if that fails.
public sealed class FfConverter : IConverter
{
    private const string AudioMap = "-map 0:a:0 -vn -map_metadata 0 ";
    private const string VideoMap = "-map 0:v:0 -map 0:a? -sn -dn -map_metadata 0 -vf \"scale=trunc(iw/2)*2:trunc(ih/2)*2\" ";
    private readonly IFfTool _tool;
    private readonly IAccel _accel;

    public FfConverter(IFfTool tool, IAccel accel, MediaKind kind)
    {
        _tool = tool;
        _accel = accel;
        Kind = kind;
    }

    public MediaKind Kind { get; }

    public void Run(MediaFile f, Target t, string dst, IProgress<double> log, CancellationToken ct)
    {
        FfTarget ft = (FfTarget)t;
        string gpu = ft.Family != null ? _accel.Video(ft.Family) : null;
        if (gpu != null)
        {
            try
            {
                Convert(f, ft, dst, gpu, "-hwaccel auto", log, ct);
                _accel.Ok(ft.Family);
                return;
            }
            catch (Exception)
            {
                ct.ThrowIfCancellationRequested();
                _accel.Fail(ft.Family);
                if (File.Exists(dst)) File.Delete(dst);
            }
        }
        Convert(f, ft, dst, ft.Soft, "", log, ct);
    }

    private void Convert(MediaFile f, FfTarget t, string dst, string video, string input, IProgress<double> log,
        CancellationToken ct)
    {
        FFMpegConverter conv = _tool.Make();
        conv.ConvertProgress += (s, e) =>
        {
            if (e.TotalDuration.TotalSeconds > 0) log.Report(Math.Min(1, e.Processed.TotalSeconds / e.TotalDuration.TotalSeconds));
        };
        ConvertSettings set = new ConvertSettings { CustomInputArgs = input.Length > 0 ? input : null, CustomOutputArgs = Args(f, t, video) };
        using (ct.Register(conv.Abort))
        {
            try
            {
                conv.ConvertMedia(f.Path, null, dst, null, set);
            }
            catch (Exception)
            {
                ct.ThrowIfCancellationRequested();
                throw;
            }
        }
        ct.ThrowIfCancellationRequested();
    }

    private string Args(MediaFile f, FfTarget t, string video)
    {
        string map = Kind == MediaKind.Audio ? AudioMap : VideoMap;
        return map + t.Args.Replace("{v}", video).Replace("{k}", Rate.For(t, f).ToString());
    }
}
