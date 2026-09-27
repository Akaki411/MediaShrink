using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MediaShrink.Core;
using MediaShrink.Estimate;
using MediaShrink.Models;
using MediaShrink.Scan;
using MediaShrink.Targets;
using MediaShrink.Tools;
using MediaShrink.Work;

namespace MediaShrink.Vm;

// Main window logic: picks folders, scans them, shows before and after, and runs the conversion.
public sealed class MainVm : Bindable
{
    private readonly IScanner _scan;
    private readonly IPlanner _plan;
    private readonly IRunner _run;
    private readonly IFolderPicker _pick;
    private readonly ITargets _targets;
    private readonly IAccel _accel;
    private readonly Grouper _grp;
    private readonly RowBuilder _rows;
    private List<MediaFile> _files = new List<MediaFile>();
    private List<Group> _groups = new List<Group>();
    private CancellationTokenSource _cts;
    private string _status = "Выберите папку с исходниками";
    private string _eta = "";
    private string _hw = "Проверка видеокарты…";
    private double _pct;
    private bool _running;
    private bool _scanning;

    public MainVm(IScanner scan, IPlanner plan, IRunner run, IFolderPicker pick, ITargets targets,
        IAccel accel, Grouper grp, RowBuilder rows)
    {
        _scan = scan;
        _plan = plan;
        _run = run;
        _pick = pick;
        _targets = targets;
        _accel = accel;
        _grp = grp;
        _rows = rows;

        Left = new SideVm();
        Right = new SideVm();
        Photo = new PickVm(MediaKind.Photo, "Фото");
        Audio = new PickVm(MediaKind.Audio, "Аудио");
        Video = new PickVm(MediaKind.Video, "Видео");
        Picks = new[] { Photo, Audio, Video };
        foreach (PickVm p in Picks) p.Changed += Redraw;

        PickSrc = new Cmd(OnPickSrc, () => Idle);
        PickDst = new Cmd(OnPickDst, () => Idle);
        Toggle = new Cmd(OnToggle, () => Running || (Idle && HasMedia));
        Redraw();
        ShowHw();
    }

    public SideVm Left { get; }
    public SideVm Right { get; }
    public PickVm Photo { get; }
    public PickVm Audio { get; }
    public PickVm Video { get; }
    public PickVm[] Picks { get; }
    public ICommand PickSrc { get; }
    public ICommand PickDst { get; }
    public ICommand Toggle { get; }

    public string Status
    {
        get { return _status; }
        private set { Set(ref _status, value); }
    }

    public string Eta
    {
        get { return _eta; }
        private set { Set(ref _eta, value); }
    }

    public string Hw
    {
        get { return _hw; }
        private set { Set(ref _hw, value); }
    }

    public double Pct
    {
        get { return _pct; }
        private set { Set(ref _pct, value); }
    }

    public bool Running
    {
        get { return _running; }
        private set
        {
            if (Set(ref _running, value)) Refresh();
        }
    }

    public bool Scanning
    {
        get { return _scanning; }
        private set
        {
            if (Set(ref _scanning, value)) Refresh();
        }
    }

    public bool Idle
    {
        get { return !_running && !_scanning; }
    }

    public string BtnText
    {
        get { return _running ? "Остановить" : "Начать"; }
    }

    private bool HasMedia
    {
        get { return _groups.Any(g => g.Kind != MediaKind.Other); }
    }

    private async void ShowHw()
    {
        Hw = await Task.Run(() => _accel.Info());
    }

    private void Refresh()
    {
        Raise("Idle");
        Raise("BtnText");
        foreach (PickVm p in Picks) p.Locked = !Idle;
        CommandManager.InvalidateRequerySuggested();
    }

    private Dictionary<MediaKind, Target> Chosen()
    {
        Dictionary<MediaKind, Target> map = new Dictionary<MediaKind, Target>();
        foreach (PickVm p in Picks)
        {
            if (p.Target != null) map[p.Kind] = p.Target;
        }
        return map;
    }

    private void Redraw()
    {
        Left.Show(_rows.Build(_groups), _files.Count.ToString("N0") + " файлов");
        Dictionary<MediaKind, Target> pick = Chosen();
        Table after = _rows.Build(_plan.After(_groups, pick));
        long before = _groups.Where(g => pick.ContainsKey(g.Kind)).Sum(g => g.Bytes);
        double delta = before > 0 ? (double)after.Bytes / before - 1 : 0;
        Right.Show(after, Fmt.Delta(delta) + " к исходным");
    }

    private string Counts()
    {
        Func<MediaKind, int> n = k => _files.Count(f => f.Kind == k);
        return "Фото " + n(MediaKind.Photo).ToString("N0") + " · Аудио " + n(MediaKind.Audio).ToString("N0")
               + " · Видео " + n(MediaKind.Video).ToString("N0") + " · Другое " + n(MediaKind.Other).ToString("N0");
    }

    private void OnPickDst()
    {
        string dir = _pick.Pick(Right.Path);
        if (dir != null) Right.Path = dir;
    }

    private async void OnPickSrc()
    {
        string dir = _pick.Pick(Left.Path);
        if (dir == null) return;

        Left.Path = dir;
        Scanning = true;
        Status = "Поиск файлов…";
        try
        {
            IProgress<string> log = new Progress<string>(s => Status = s);
            List<MediaFile> files = await _scan.Run(dir, log, CancellationToken.None);
            List<Group> groups = _grp.Make(files);

            Dictionary<MediaKind, List<Target>> lists = new Dictionary<MediaKind, List<Target>>();
            Dictionary<Target, double> deltas = null;
            await Task.Run(() =>
            {
                foreach (PickVm p in Picks) lists[p.Kind] = _targets.For(p.Kind);
                deltas = _plan.Deltas(files, lists.Values.SelectMany(x => x));
            });

            _files = files;
            _groups = groups;
            foreach (PickVm p in Picks) p.Load(lists[p.Kind], deltas, groups.Any(g => g.Kind == p.Kind));
            Left.Info = files.Count.ToString("N0") + " файлов · " + Fmt.Size(files.Sum(f => f.Size)) + "\n" + Counts();
            Redraw();
            Status = HasMedia ? "Выберите форматы и папку результата" : "В папке нет фото, аудио и видео";
        }
        catch (Exception e)
        {
            Status = "Ошибка: " + e.Message;
        }
        finally
        {
            Scanning = false;
        }
    }

    private async void OnToggle()
    {
        if (Running)
        {
            _cts.Cancel();
            Status = "Останавливаю…";
            return;
        }

        string src = Left.Path;
        string dst = Right.Path;
        if (string.IsNullOrEmpty(dst))
        {
            Status = "Выберите папку для результата";
            return;
        }
        if (string.Equals(src.TrimEnd('\\'), dst.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
        {
            Status = "Папка результата должна отличаться от исходной";
            return;
        }

        Dictionary<MediaKind, Target> pick = Chosen();
        string skip = dst.TrimEnd('\\') + "\\";
        List<MediaFile> todo = _files
            .Where(f => pick.ContainsKey(f.Kind) && !f.Path.StartsWith(skip, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (todo.Count == 0) return;

        _cts = new CancellationTokenSource();
        CancellationToken ct = _cts.Token;
        Pct = 0;
        Eta = "";
        Running = true;
        Progress<Step> log = new Progress<Step>(s =>
        {
            Pct = s.Done * 100;
            Eta = (int)(s.Done * 100) + " % · " + (s.Left.HasValue ? "осталось " + Fmt.Span(s.Left.Value) : "оценка времени…");
            Status = "Сейчас: " + s.Now;
        });

        try
        {
            Outcome res = await Task.Run(() => _run.Run(todo, pick, src, dst, log, ct));
            Status = Summary(res);
        }
        catch (Exception e)
        {
            Status = "Ошибка: " + e.Message;
        }
        finally
        {
            Eta = "";
            Running = false;
            ShowHw();
        }
    }

    private static string Summary(Outcome res)
    {
        string head = res.Stopped ? "Остановлено" : "Готово";
        string text = head + " · файлов: " + res.Ok.ToString("N0");
        if (res.In > 0) text += " · " + Fmt.Size(res.In) + " → " + Fmt.Size(res.Out) + " (" + Fmt.Delta((double)res.Out / res.In - 1) + ")";
        if (res.Fail > 0) text += " · ошибок: " + res.Fail;
        return text;
    }
}

