using RsfRbrPowerSteering.ViewModel.Interfaces;
using System.Windows.Input;

namespace RsfRbrPowerSteering.Implementations;

internal class CommandManagerWrapper : ICommandManager
{
    public event EventHandler RequerySuggested
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void InvalidateRequerySuggested()
        => CommandManager.InvalidateRequerySuggested();
}
