using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ResetToDefaultsCommand(
    ICommandManager commandManager,
    MainViewModel mainViewModel)
    : ExclusiveAsyncCommandBase(
        commandManager,
        mainViewModel)
{
    protected override Task ExecuteExclusiveAsync(object? parameter)
    {
        MainViewModel.ResetToDefaults();

        return Task.CompletedTask;
    }
}
