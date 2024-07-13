using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ReloadCarsCommand(
    ICommandManager commandManager,
    MainViewModel mainViewModel)
    : ExclusiveAsyncCommandBase(
        commandManager,
        mainViewModel)
{
    protected override async Task ExecuteExclusiveAsync(object? parameter)
        => await MainViewModel.LoadCarsAsync();
}
