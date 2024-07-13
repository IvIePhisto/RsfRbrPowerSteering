namespace RsfRbrPowerSteering.ViewModel;

public class LockToLockRotationViewModel
{
    internal LockToLockRotationViewModel(int lockToLockRotation)
    {
        IntValue = lockToLockRotation;
        DoubleValue = Convert.ToDouble(lockToLockRotation);
        Text = $"{lockToLockRotation}";
    }

    public int IntValue { get; }
    public double DoubleValue { get; }
    public string Text { get; }
}
