namespace RsfRbrPowerSteering.Model.Calculation;

public struct DrivetrainFactors(decimal fwd, decimal rwd, decimal awd)
{
    public decimal Fwd { get; } = fwd;
    public decimal Rwd { get; } = rwd;
    public decimal Awd { get; } = awd;

    public decimal this[Drivetrain drivetrain]
        => drivetrain switch
        {
            Drivetrain.Fwd => Fwd,
            Drivetrain.Rwd => Rwd,
            Drivetrain.Awd => Awd,
            _ => throw new InvalidOperationException($"Unexpected {typeof(Drivetrain)} value: {drivetrain}")
        };
}
