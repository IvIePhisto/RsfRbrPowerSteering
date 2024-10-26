using RsfRbrPowerSteering.ViewModel.Interfaces;
using System.Windows.Input;

namespace RsfRbrPowerSteering.ViewModel.Commands;

public class ViewModelCommands
{
    internal ViewModelCommands(
        ICommandManager commandManager,
        IMessageService messageService,
        MainViewModel mainViewModel)
    {
        SetPrimarySurfaceNull = new SetPrimarySurfaceNullCommand(
            commandManager,
            mainViewModel);
        Load = new LoadCommand(
            commandManager,
            mainViewModel);
        ReloadCars = new ReloadCarsCommand(
            commandManager,
            mainViewModel);
        ExportCars = new ExportCarsCommand(
            commandManager,
            messageService,
            mainViewModel);
        ImportCars = new ImportCarsCommand(
            commandManager,
            messageService,
            mainViewModel);
        Exit = new ExitCommand(
            commandManager,
            mainViewModel);
        ApplyScaling = new ApplyScalingCommand(
            commandManager,
            messageService,
            mainViewModel);
        ResetToDefaults = new ResetToDefaultsCommand(
            commandManager,
            mainViewModel);
        ClearFfbSens = new ClearFfbSensCommand(
            commandManager,
            messageService,
            mainViewModel);
        ToggleDescriptionVisibility = new ToggleDescriptionVisibilityCommand(
            mainViewModel);
    }

    public ICommand SetPrimarySurfaceNull { get; }
    public ICommand Load { get; }
    public ICommand ReloadCars { get; }
    public ICommand ExportCars { get; }
    public ICommand ImportCars { get; }
    public ICommand Exit { get; }
    public ICommand ApplyScaling { get; }
    public ICommand ResetToDefaults { get; }
    public ICommand ClearFfbSens { get; }
    public ICommand ToggleDescriptionVisibility { get; }
}
