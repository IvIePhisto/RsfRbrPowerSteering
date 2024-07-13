namespace RsfRbrPowerSteering.ViewModel;

public interface IMessageService
{
    void ShowErrorMessage(string message);
    void ShowExceptionError(Exception exception);
    bool Ask(string question);
    FileInfo? ChooseExistingFile(DirectoryInfo initialDirectory, string title, string filter, string defaultExtension);
    FileInfo? ChooseSaveFile(DirectoryInfo initialDirectory, string title, string filter, string defaultExtension, string fileName);
}
