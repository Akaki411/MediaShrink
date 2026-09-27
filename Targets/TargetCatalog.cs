using System.Collections.Generic;
using System.Linq;
using MediaShrink.Models;
using MediaShrink.Tools;

namespace MediaShrink.Targets;

// Collects all format lists and keeps only the formats the installed tools can write.
public sealed class TargetCatalog : ITargets
{
    private readonly IEnumerable<ITargetSource> _sources;
    private readonly IEncoders _enc;
    private readonly Dictionary<MediaKind, List<Target>> _cache = new Dictionary<MediaKind, List<Target>>();

    public TargetCatalog(IEnumerable<ITargetSource> sources, IEncoders enc)
    {
        _sources = sources;
        _enc = enc;
    }

    public List<Target> For(MediaKind kind)
    {
        lock (_cache)
        {
            List<Target> list;
            if (!_cache.TryGetValue(kind, out list))
            {
                list = _sources.Where(s => s.Kind == kind).SelectMany(s => s.All()).Where(t => t.Ready(_enc)).ToList();
                _cache[kind] = list;
            }
            return list;
        }
    }
}
