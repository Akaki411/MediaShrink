using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MediaShrink.Models;

namespace MediaShrink.Tools;

// Finds a working GPU encoder by test-encoding a short clip. After 3 errors in a row a codec goes back to the CPU.
public sealed class FfAccel : IAccel
{
    private const int Limit = 3;
    private static readonly string[] Families = { "h264", "hevc" };
    private readonly IFfTool _tool;
    private readonly IEncoders _enc;
    private readonly Lazy<Dictionary<string, string>> _found;
    private readonly Dictionary<string, int> _fails = new Dictionary<string, int>();

    public FfAccel(IFfTool tool, IEncoders enc)
    {
        _tool = tool;
        _enc = enc;
        _found = new Lazy<Dictionary<string, string>>(Detect);
    }

    public string Video(string family)
    {
        string enc;
        if (!_found.Value.TryGetValue(family, out enc)) return null;
        lock (_fails)
        {
            int n;
            if (_fails.TryGetValue(family, out n) && n >= Limit) return null;
        }
        return HwArgs.For(enc);
    }

    public void Ok(string family)
    {
        lock (_fails) _fails[family] = 0;
    }

    public void Fail(string family)
    {
        lock (_fails)
        {
            int n;
            _fails.TryGetValue(family, out n);
            _fails[family] = n + 1;
        }
    }

    public string Info()
    {
        Dictionary<string, string> map = _found.Value;
        List<string> live = Families.Where(f => Video(f) != null).ToList();
        if (live.Count == 0) return "Аппаратное ускорение не найдено — кодирование на процессоре";
        IEnumerable<string> parts = live.GroupBy(f => HwArgs.Vendor(map[f]))
            .Select(g => g.Key + " (" + string.Join(", ", g.Select(Codecs.Nice)) + ")");
        return "Аппаратное ускорение: " + string.Join(" · ", parts) + " — включается автоматически";
    }

    private Dictionary<string, string> Detect()
    {
        Dictionary<string, string> map = new Dictionary<string, string>();
        foreach (string family in Families)
        {
            foreach (string enc in HwArgs.Order(family))
            {
                if (!_enc.Has(enc) || !Works(enc)) continue;
                map[family] = enc;
                break;
            }
        }
        return map;
    }

    private bool Works(string enc)
    {
        try
        {
            string args = "-hide_banner -loglevel error -f lavfi -i color=size=640x360:rate=10:duration=0.5 "
                          + HwArgs.For(enc) + " -f null -";
            ProcessStartInfo info = new ProcessStartInfo(_tool.Exe(), args) { UseShellExecute = false, CreateNoWindow = true };
            using (Process proc = Process.Start(info))
            {
                if (proc.WaitForExit(20000)) return proc.ExitCode == 0;
                proc.Kill();
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}
