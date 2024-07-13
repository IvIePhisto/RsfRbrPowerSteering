namespace RsfRbrPowerSteering.ViewModel.Interfaces;

public interface ICommandManager
{
    event EventHandler RequerySuggested;
    void InvalidateRequerySuggested();
}
