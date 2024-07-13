using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RsfRbrPowerSteering.View;

/// <summary>
/// Interaction logic for ProgressTextBoxUserControl.xaml
/// </summary>
public partial class IntProgressTextBox : UserControl
{
    public IntProgressTextBox()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.Register(
        nameof(ToolTipText),
        typeof(string),
        typeof(IntProgressTextBox));

    public string ToolTipText
    {
        get => (string)GetValue(ToolTipTextProperty);
        set => SetValue(ToolTipTextProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        name: nameof(Value),
        propertyType: typeof(string),
        ownerType: typeof(IntProgressTextBox),
        typeMetadata: new FrameworkPropertyMetadata(
            defaultValue: null,
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            propertyChangedCallback: new PropertyChangedCallback(IntValueChanged),
            coerceValueCallback: null,
            isAnimationProhibited: false,
            defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    private static void IntValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (IntProgressTextBox)d;
        self.ProgressIntValue = Convert.ToInt32(self.Value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ProgressIntValueProperty = DependencyProperty.Register(
        name: nameof(ProgressIntValue),
        propertyType: typeof(int),
        ownerType: typeof(IntProgressTextBox),
        typeMetadata: new FrameworkPropertyMetadata(
            defaultValue: 0,
            flags: default,
            propertyChangedCallback: null,
            coerceValueCallback: null,
            isAnimationProhibited: false,
            defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    public int ProgressIntValue
    {
        get => (int)GetValue(ProgressIntValueProperty);
        private set => SetValue(ProgressIntValueProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(int),
        typeof(IntProgressTextBox));

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(int),
        typeof(IntProgressTextBox));

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
}
