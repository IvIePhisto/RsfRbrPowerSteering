using RsfRbrPowerSteering.Implementations;
using RsfRbrPowerSteering.Model;
using RsfRbrPowerSteering.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace RsfRbrPowerSteering.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MessageBoxUtility = new MessageBoxService(this);
        var viewModel = new MainViewModel(
            new CommandManagerWrapper(),
            MessageBoxUtility);
        DataContext = viewModel;
    }

    internal MessageBoxService MessageBoxUtility { get; }
}