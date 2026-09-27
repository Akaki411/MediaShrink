using System.Windows;
using MediaShrink.Estimate;
using MediaShrink.Models;
using MediaShrink.Scan;
using MediaShrink.Targets;
using MediaShrink.Tools;
using MediaShrink.Views;
using MediaShrink.Vm;
using MediaShrink.Work;

namespace MediaShrink;

// Program start: wires all parts together and opens the main window.
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        FfTool tool = new FfTool();
        FfEncoders encoders = new FfEncoders(tool);
        FfAccel accel = new FfAccel(tool, encoders);
        TargetCatalog targets = new TargetCatalog(
            new ITargetSource[] { new PhotoTargets(), new AudioTargets(), new VideoTargets() }, encoders);
        Planner plan = new Planner(new IEstimator[] { new PhotoEstimator(), new AudioEstimator(), new VideoEstimator(accel) });
        Runner run = new Runner(
            new IConverter[]
            {
                new PhotoConverter(),
                new FfConverter(tool, accel, MediaKind.Audio),
                new FfConverter(tool, accel, MediaKind.Video)
            },
            new PathMaker());
        FolderScanner scan = new FolderScanner(new KindMap(), new FfProbe(tool));

        MainVm vm = new MainVm(scan, plan, run, new FolderPicker(), targets, accel, new Grouper(), new RowBuilder());
        MainWindow win = new MainWindow { DataContext = vm };
        win.Show();
    }
}

