using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal abstract class ExclusiveAsyncCommandBase(
    ICommandManager commandManager,
    MainViewModel mainViewModel)
    : AsyncCommandBase(commandManager)
{
    protected MainViewModel MainViewModel { get; } = mainViewModel;

    public override bool CanExecute(object? parameter)
        => !MainViewModel.IsExclusiveCommandRunning;

    protected abstract Task ExecuteExclusiveAsync(object? parameter);

    public override async Task ExecuteAsync(object? parameter)
    {
        MainViewModel.IsExclusiveCommandRunning = true;
        await ExecuteExclusiveAsync(parameter);
        MainViewModel.IsExclusiveCommandRunning = false;

        RaiseCanExecuteChanged();
    }
}
