using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class SetPrimarySurfaceNullCommand(
    ICommandManager commandManager,
    MainViewModel mainViewModel)
    : ExclusiveAsyncCommandBase(
        commandManager,
        mainViewModel)
{
    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter)
            && MainViewModel.Adjustments.IsPrimarySurfaceSet;
    }

    protected override Task ExecuteExclusiveAsync(object? parameter)
    {
        MainViewModel.Adjustments.IsPrimarySurfaceNull = true;

        return Task.CompletedTask;
    }
}
