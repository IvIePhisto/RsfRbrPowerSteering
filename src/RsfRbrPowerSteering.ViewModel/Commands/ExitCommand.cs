using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ExitCommand(
    ICommandManager commandManager,
    MainViewModel mainViewModel)
    : ExclusiveAsyncCommandBase(
        commandManager,
        mainViewModel)
{
    protected override async Task ExecuteExclusiveAsync(object? parameter)
        => await MainViewModel.SaveSettingsAsync();
}
