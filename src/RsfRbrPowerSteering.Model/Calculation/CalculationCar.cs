namespace RsfRbrPowerSteering.Model.Calculation;

public readonly struct CalculationCar(
    int maxSteeringLock,
    int weightKg,
    CarFfbSens ffbSens,
    Drivetrain drivetrain)
{
    public int MaxSteeringLock { get; } = maxSteeringLock;
    public int WeightKg { get; } = weightKg;
    public CarFfbSens FfbSens { get; } = ffbSens;
    public Drivetrain Drivetrain { get; } = drivetrain;
}
