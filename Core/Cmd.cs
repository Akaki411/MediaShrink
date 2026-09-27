using System;
using System.Windows.Input;

namespace MediaShrink.Core;

// Minimal ICommand: runs an action and can be switched off by a condition.
public sealed class Cmd : ICommand
{
    private readonly Action _run;
    private readonly Func<bool> _can;

    public Cmd(Action run, Func<bool> can = null)
    {
        _run = run;
        _can = can;
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object arg)
    {
        return _can == null || _can();
    }

    public void Execute(object arg)
    {
        _run();
    }
}
