using RsfRbrPowerSteering.ViewModel;
using System.Windows.Controls;

namespace RsfRbrPowerSteering.View;

/// <summary>
/// Interaction logic for Preview.xaml
/// </summary>
public partial class Preview : UserControl
{
    public Preview()
    {
        InitializeComponent();
    }

    private void FilterCarIsNotDefault(object sender, System.Windows.Data.FilterEventArgs e)
        => e.Accepted = (e.Item as CarViewModel)?.Id != 0;
}
