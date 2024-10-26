using System.Windows.Input;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ToggleDescriptionVisibilityCommand(MainViewModel mainViewModel)
    : ICommand
{
    private readonly MainViewModel _mainViewModel = mainViewModel;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
        => true;

    public void Execute(object? parameter)
        => _mainViewModel.IsDescriptionVisible = !_mainViewModel.IsDescriptionVisible;
}
