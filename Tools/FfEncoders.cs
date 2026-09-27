using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MediaShrink.Tools;

// Reads the encoder list from ffmpeg once. If that fails, every encoder counts as present.
public sealed class FfEncoders : IEncoders
{
    private static readonly Regex Line = new Regex(@"^\s*[VAS][\.\w]{5}\s+(\S+)", RegexOptions.Compiled);
    private readonly Lazy<HashSet<string>> _names;

    public FfEncoders(IFfTool tool)
    {
        _names = new Lazy<HashSet<string>>(() => Load(tool));
    }

    public bool Has(string name)
    {
        HashSet<string> set = _names.Value;
        return set == null || set.Contains(name);
    }

    private static HashSet<string> Load(IFfTool tool)
    {
        try
        {
            ProcessStartInfo info = new ProcessStartInfo(tool.Exe(), "-hide_banner -encoders")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            HashSet<string> set = new HashSet<string>();
            using (Process proc = Process.Start(info))
            {
                string text = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                foreach (string line in text.Split('\n'))
                {
                    Match m = Line.Match(line);
                    if (m.Success) set.Add(m.Groups[1].Value);
                }
            }
            return set.Count > 0 ? set : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
