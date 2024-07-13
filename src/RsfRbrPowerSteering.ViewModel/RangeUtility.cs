namespace RsfRbrPowerSteering.ViewModel;

internal static class RangeUtility
{
    public static void EnsureRange(ref int value, int minimum, int maximum)
    {
        if (value < minimum)
        {
            value = minimum;
        }

        if (value > maximum)
        {
            value = maximum;
        }
    }

    public static void EnsureRange(ref int? value, int minimum, int maximum)
    {
        if (!value.HasValue)
        {
            return;
        }

        int localValue = value.Value;
        EnsureRange(ref localValue, minimum, maximum);
        value = localValue;
    }

    public static void EnsureRange(ref double value, double minimum, double maximum)
    {
        if (value < minimum)
        {
            value = minimum;
        }

        if (value > maximum)
        {
            value = maximum;
        }
    }
}
