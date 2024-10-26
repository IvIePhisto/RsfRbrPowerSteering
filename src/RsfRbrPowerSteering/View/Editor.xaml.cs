using RsfRbrPowerSteering.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RsfRbrPowerSteering.View;

/// <summary>
/// Interaction logic for Editor.xaml
/// </summary>
public partial class Editor : UserControl
{
    public Editor()
    {
        InitializeComponent();
        UpdateLockToLockRotationsForSlider();
        SubscribeToLockToLockRotationChanges();
        DataContextChanged += (sender, e) => SubscribeToLockToLockRotationChanges();
    }

    private void UpdateLockToLockRotationsForSlider()
    {
        if (DataContext is MainViewModel viewModel)
        {
            LockToLockRotationsForSlider = new DoubleCollection(
                    viewModel.LockToLockRotations
                        .Select(r => r.DoubleValue)
                        .Order());
        }
    }

    private void SubscribeToLockToLockRotationChanges()
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.LockToLockRotationsChanged += UpdateLockToLockRotationsForSlider;
        }
    }

    public static readonly DependencyProperty LockToLockRotationsForSliderDependencyProperty = DependencyProperty.Register(
        nameof(LockToLockRotationsForSlider),
        typeof(DoubleCollection),
        typeof(Editor));

    public DoubleCollection LockToLockRotationsForSlider
    {
        get => (DoubleCollection)GetValue(LockToLockRotationsForSliderDependencyProperty);
        private set => SetValue(LockToLockRotationsForSliderDependencyProperty, value);
    }

    private void Ratio_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        var mainViewModel = DataContext as MainViewModel;

        if (mainViewModel == null)
        {
            return;
        }

        switch (e.Key)
        {
            case System.Windows.Input.Key.Up:
                mainViewModel.Adjustments.LockToLockRotationRatio++;

                break;

            case System.Windows.Input.Key.Down:
                mainViewModel.Adjustments.LockToLockRotationRatio--;

                break;
        }
    }

    private void TargetCar_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var mainViewModel = DataContext as MainViewModel;

        if (mainViewModel == null || mainViewModel.TargetCar == null)
        {
            return;
        }

        var mainWindow = Application.Current.MainWindow as MainWindow;

        if (mainWindow == null)
        {
            return;
        }

        mainWindow.Preview.CarPreviews.ScrollIntoView(mainViewModel.TargetCar);
    }
}
