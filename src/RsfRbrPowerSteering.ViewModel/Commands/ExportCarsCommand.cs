using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ExportCarsCommand(
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
        FileInfo? file = _messageService.ChooseSaveFile(
            new DirectoryInfo("."),
            ViewModelTexts.ExportCarsSaveFileDialogFileName,
            ViewModelTexts.CarsFileDialogFilter,
            ".json",
            ViewModelTexts.ExportCarsSaveFileDialogTitle);

        if (file == null)
        {
            return;
        }

        await MainViewModel.ExportCarsAsync(file);
    }
}
