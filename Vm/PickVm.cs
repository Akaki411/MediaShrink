using System;
using System.Collections.Generic;
using System.Linq;
using MediaShrink.Core;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Vm;

// The drop-down for one media kind. It is off when the folder has no files of that kind.
public sealed class PickVm : Bindable
{
    private List<Choice> _items = new List<Choice>();
    private Choice _sel;
    private bool _has;
    private bool _locked;

    public PickVm(MediaKind kind, string title)
    {
        Kind = kind;
        Title = title;
    }

    public event Action Changed;

    public MediaKind Kind { get; }
    public string Title { get; }

    public List<Choice> Items
    {
        get { return _items; }
        private set { Set(ref _items, value); }
    }

    public Choice Sel
    {
        get { return _sel; }
        set
        {
            if (Set(ref _sel, value) && Changed != null) Changed();
        }
    }

    public bool On
    {
        get { return _has && !_locked; }
    }

    public bool Has
    {
        get { return _has; }
    }

    public bool Locked
    {
        set
        {
            _locked = value;
            Raise("On");
        }
    }

    public Target Target
    {
        get { return _has && _sel != null ? _sel.Target : null; }
    }

    public void Load(List<Target> targets, Dictionary<Target, double> deltas, bool has)
    {
        string keep = _sel != null ? _sel.Target.Id : null;
        List<Choice> list = targets.Select(t => new Choice(t)).ToList();
        foreach (Choice c in list) c.Show(deltas.ContainsKey(c.Target) ? deltas[c.Target] : 0);
        _items = list;
        Raise("Items");
        _sel = list.FirstOrDefault(c => c.Target.Id == keep) ?? list.FirstOrDefault();
        Raise("Sel");
        _has = has;
        Raise("Has");
        Raise("On");
    }
}
