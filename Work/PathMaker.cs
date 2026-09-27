using System.Collections.Generic;
using System.IO;
using MediaShrink.Models;

namespace MediaShrink.Work;

// Builds the result path: same subfolders as the source, new extension, no name clashes.
public sealed class PathMaker
{
    public string Make(string src, string dst, MediaFile f, string ext, ISet<string> used)
    {
        string rel = f.Path.Substring(src.TrimEnd('\\').Length).TrimStart('\\');
        string dir = Path.GetDirectoryName(rel) ?? "";
        string name = Path.GetFileNameWithoutExtension(rel);
        string path = Path.Combine(dst, dir, name + ext);
        int n = 1;
        while (!used.Add(path))
        {
            path = Path.Combine(dst, dir, name + " (" + n + ")" + ext);
            n++;
        }
        return path;
    }
}
