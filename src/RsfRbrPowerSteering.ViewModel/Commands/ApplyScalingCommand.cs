using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class ApplyScalingCommand(
    ICommandManager commandManager,
    IMessageService messageService,
    MainViewModel mainViewModel)
    : ExclusiveAsyncCommandBase(
        commandManager,
        mainViewModel)
{
    private readonly IMessageService _messageService = messageService;

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter)
            && MainViewModel.PrimaryTemplate.FfbSensesHaveValue
            && MainViewModel.SecondaryTemplate.FfbSensesHaveValue;
    }

    protected override async Task ExecuteExclusiveAsync(object? parameter)
    {
        string question =
            MainViewModel.TargetCar == null
            ? ViewModelTexts.ApplyScalingConfirmationAllCars
            : string.Format(
                ViewModelTexts.ApplyScalingConfirmationSingleCarFormat,
                MainViewModel.TargetCar.Name);

        if (!_messageService.Ask(question))
        {
            return;
        }

        await MainViewModel.ApplyScalingAsync();
    }
}
