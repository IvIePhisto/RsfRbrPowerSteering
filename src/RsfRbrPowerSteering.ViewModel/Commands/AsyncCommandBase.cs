using RsfRbrPowerSteering.ViewModel.Interfaces;
using System.Windows.Input;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal abstract class AsyncCommandBase : NotifyPropertyChangedBase, ICommand
{
    private bool _isRunning;
    private readonly ICommandManager _commandManager;

    public AsyncCommandBase(ICommandManager commandManager)
    {
        _commandManager = commandManager;
    }

    public bool IsRunning
    {
        get => _isRunning;
        protected set
        {
            _isRunning = value;
            NotifyPropertyChanged();
        }
    }

    public abstract bool CanExecute(object? parameter);
    public abstract Task ExecuteAsync(object? parameter);

    public async void Execute(object? parameter)
    {
        IsRunning = true;
        await ExecuteAsync(parameter);
        IsRunning = false;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => _commandManager.RequerySuggested += value;
        remove => _commandManager.RequerySuggested -= value;
    }

    public void RaiseCanExecuteChanged()
        => _commandManager.InvalidateRequerySuggested();
}
