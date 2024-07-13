using RsfRbrPowerSteering.Model;

namespace RsfRbrPowerSteering.Settings;

public class CarSettings
{
    public int? SelectedCarId { get; set; }
    public int WeightKg { get; set; }
    public int LockToLockRotation { get; set; }
    public Drivetrain Drivetrain { get; set; }
    public int? FfbSensGravel { get; set; }
    public int? FfbSensTarmac { get; set; }
    public int? FfbSensSnow { get; set; }
}
