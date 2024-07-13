using RsfRbrPowerSteering.Implementations;
using RsfRbrPowerSteering.Model.Rsf;
using RsfRbrPowerSteering.View;
using System.Diagnostics;
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
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => view.MessageBoxUtility.ShowExceptionError((Exception)args.ExceptionObject);
        TaskScheduler.UnobservedTaskException += (sender, args) => view.MessageBoxUtility.ShowExceptionError(args.Exception);
        DispatcherUnhandledException += (sender, args) =>
        {
            if (!Debugger.IsAttached)
            {
                args.Handled = true;
                view.MessageBoxUtility.ShowExceptionError(args.Exception);
            }
        };

        // Show window:
        view.Show();
    }
}
