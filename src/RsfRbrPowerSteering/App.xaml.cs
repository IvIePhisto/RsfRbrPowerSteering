using RsfRbrPowerSteering.Implementations;
using RsfRbrPowerSteering.Model.Rsf;
using RsfRbrPowerSteering.Settings;
using RsfRbrPowerSteering.View;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace RsfRbrPowerSteering;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs eventArgs)
    {
        if (!RsfFiles.Check(out IReadOnlyList<string> missingFilePaths))
        {
            MessageBoxService.ShowMissingFilesError(missingFilePaths.ToArray());
            Shutdown();

            return;
        }

        var view = new MainWindow();

        // Catch unhandled exceptions:
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => HandleException(view, (Exception)args.ExceptionObject);
        TaskScheduler.UnobservedTaskException += (sender, args) => HandleException(view, args.Exception);
        DispatcherUnhandledException += (sender, args) =>
        {
            HandleException(view, args.Exception);
            args.Handled = true;
        };

        // Show window:
        view.Show();
    }

    /// <remarks>This is intentionally fire-and-forget.</remarks>
    private async void LogExceptionAsync(Exception exception)
    {
        try
        {
            using var exceptionZipArchiveFileStream = File.Create($@".\{nameof(RsfRbrPowerSteering)} Exception.zip");
            using var execeptionZipArchive = new ZipArchive(exceptionZipArchiveFileStream, ZipArchiveMode.Create);

            async Task CreateExceptionEntryAsync(Exception entryException, string fileName)
            {
                ZipArchiveEntry exceptionEntry = execeptionZipArchive.CreateEntry(fileName);

                using Stream exceptionEntryStream = exceptionEntry.Open();
                using var exceptionEntryWriter = new StreamWriter(exceptionEntryStream);
                await exceptionEntryWriter.WriteAsync(entryException.ToString());
            }

            await CreateExceptionEntryAsync(exception, "Exception.txt");
            string currentDirectoryPath = Directory.GetCurrentDirectory();

            async Task CreateEntryAsync(FileInfo file)
            {
                if (file.Exists)
                {
                    string entryName = file.FullName[(currentDirectoryPath.Length + 1)..];

                    try
                    {
                        ZipArchiveEntry entry = execeptionZipArchive.CreateEntryFromFile(file.FullName, entryName);
                    }
                    catch (Exception ex) {
                        await CreateExceptionEntryAsync(ex, $"{entryName}.Exception.txt");
                    }
                }
            }

            await CreateEntryAsync(RsfFiles.PersonalRsfIniFile);
            await CreateEntryAsync(RsfFiles.CarsDataFile);
            await CreateEntryAsync(RsfFiles.CarsFile);
            await CreateEntryAsync(RootSettings.SettingsFile);
        }
        catch
        {
            // Nothing else to do, so exception is ignored.
        }
    }

    private void HandleException(MainWindow view, Exception exception)
    {
        LogExceptionAsync(exception);
        view.MessageBoxUtility.ShowExceptionError(exception);
    }
}
