using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ImportCarsCommand(
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
        FileInfo? file = _messageService.ChooseExistingFile(
            new DirectoryInfo("."),
            ViewModelTexts.ImportCarsSaveFileDialogTitle,
            ViewModelTexts.CarsFileDialogFilter,
            ".json");

        if (file == null)
        {
            return;
        }

        if (!_messageService.Ask(ViewModelTexts.ImportCarsConfirmation))
        {
            return;
        }

        await MainViewModel.ImportCarsAsync(file);
    }    
}
