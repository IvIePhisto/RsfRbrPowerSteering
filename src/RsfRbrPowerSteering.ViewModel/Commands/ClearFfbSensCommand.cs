using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ClearFfbSensCommand(
    ICommandManager commandManager,
    IMessageService messageService,
    MainViewModel mainViewModel)
    : ExclusiveAsyncCommandBase(
        commandManager,
        mainViewModel)
{
    private readonly IMessageService _messageService = messageService;

    protected override async Task ExecuteExclusiveAsync(object? parameter)
    {
        if (_messageService.Ask(ViewModelTexts.ClearFfbSensQuestion))
        {
            await MainViewModel.ClearFfbSensAsync();
        }
    }
}
