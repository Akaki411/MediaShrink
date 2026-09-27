using System.Collections.Generic;
using MediaShrink.Models;

namespace MediaShrink.Targets;

// A list of formats for one media kind.
public interface ITargetSource
{
    MediaKind Kind { get; }
    IEnumerable<Target> All();
}
