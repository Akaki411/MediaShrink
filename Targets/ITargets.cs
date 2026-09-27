using System.Collections.Generic;
using MediaShrink.Models;

namespace MediaShrink.Targets;

// Gives the formats that can really be written for a media kind.
public interface ITargets
{
    List<Target> For(MediaKind kind);
}
