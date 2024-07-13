using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class LoadCommand(
    ICommandManager commandManager,
    MainViewModel mainViewModel)
    : ExclusiveAsyncCommandBase(
        commandManager,
        mainViewModel)
{
    private bool _wasLoaded;

    public override bool CanExecute(object? parameter)
        => !_wasLoaded && !MainViewModel.IsExclusiveCommandRunning;

    protected override async Task ExecuteExclusiveAsync(object? parameter)
    {
        await MainViewModel.LoadCarsAsync(false);
        await MainViewModel.LoadSettingsAsync();
        _wasLoaded = true;
    }
}
