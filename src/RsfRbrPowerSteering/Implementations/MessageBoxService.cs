using Microsoft.Win32;
using RsfRbrPowerSteering.Model.Rsf;
using RsfRbrPowerSteering.Settings;
using RsfRbrPowerSteering.View;
using RsfRbrPowerSteering.ViewModel;
using System.IO;
using System.Windows;

namespace RsfRbrPowerSteering.Implementations;

internal class MessageBoxService : IMessageService
{
    public static void ShowMissingFilesError(params string[] missingFilePaths)
        => ShowError(string.Format(
            ViewTexts.MissingFilesError,
            Path.GetFullPath("."),
            $"- {string.Join("\n- ", missingFilePaths)}"));

    public static void ShowError(string message)
        => MessageBox.Show(message, ViewTexts.WindowTitle, MessageBoxButton.OK, MessageBoxImage.Error);

    private readonly Window _owner;

    public MessageBoxService(Window owner)
    {
        _owner = owner;
    }

    public void ShowErrorMessage(string message)
        => MessageBox.Show(_owner, message, ViewTexts.WindowTitle, MessageBoxButton.OK, MessageBoxImage.Error);

    public void ShowExceptionError(Exception exception)
        => ShowErrorMessage(exception is PersonalDataException || exception is SettingsException
            ? exception.Message
            : string.Format(
                ViewTexts.UnexpectedExceptionErrorFormat,
                exception.Message));

    public bool Ask(string question)
        => MessageBox.Show(question, ViewTexts.WindowTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public FileInfo? ChooseExistingFile(DirectoryInfo initialDirectory, string title, string filter, string defaultExtension)
    {
        var openFileDialog = new OpenFileDialog
        {
            CheckPathExists = true,
            DefaultExt = defaultExtension,
            InitialDirectory = initialDirectory.FullName,
            Filter = filter,
            CheckFileExists = true,
            Title = title
        };

        return openFileDialog.ShowDialog() == true
            ? new FileInfo(openFileDialog.FileName)
            : null;
    }

    public FileInfo? ChooseSaveFile(DirectoryInfo initialDirectory, string title, string filter, string defaultExtension, string fileName)
    {
        var saveFileDialog = new SaveFileDialog
        {
            CheckPathExists = true,
            DefaultExt = defaultExtension,
            InitialDirectory = initialDirectory.FullName,
            Filter = filter,
            FileName = fileName,
            OverwritePrompt = true,
            Title = title
        };

        return saveFileDialog.ShowDialog() == true
            ? new FileInfo(saveFileDialog.FileName)
            : null;
    }
}
