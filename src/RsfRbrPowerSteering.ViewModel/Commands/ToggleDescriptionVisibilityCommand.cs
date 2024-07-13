using System.Windows.Input;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ToggleDescriptionVisibilityCommand(MainViewModel mainViewModel)
    : ICommand
{
    private readonly MainViewModel _mainViewModel = mainViewModel;

#pragma warning disable CS0067 // The event 'ToggleDescriptionVisibilityCommand.CanExecuteChanged' is never used.
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067 // The event 'ToggleDescriptionVisibilityCommand.CanExecuteChanged' is never used.

    public bool CanExecute(object? parameter)
        => true;

    public void Execute(object? parameter)
        => _mainViewModel.ToggleDescriptionVisibility();
}
