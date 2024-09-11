using RsfRbrPowerSteering.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RsfRbrPowerSteering.ViewModel;

public class CarViewModel : NotifyPropertyChangedBase
{
    private int _lockToLockRotation;
    private string _description;

    internal CarViewModel()
    {
        Id = 0;
        Name = string.Empty;
        WeightKg = 0;
        _description = string.Empty;
        LockToLockRotation = 0;
        Drivetrain = default;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal CarViewModel(CarInfo car)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        Id = car.Id;
        Name = car.Name;
        WeightKg = car.WeightKg;
        LockToLockRotation = car.LockToLockRotation;
        Drivetrain = car.Drivetrain;
        UpdateDescription();
    }

    public int Id { get; }
    public string Name { get; }
    public int WeightKg { get; }
    public Drivetrain Drivetrain { get; }

    public FfbSensViewModel FfbSensPersonal { get; } = new FfbSensViewModel();

    public FfbSensViewModel FfbSensCalculated { get; } = new FfbSensViewModel();

    private void UpdateDescription()
        => Description = $"{Name} [{LockToLockRotation}° {WeightKg}kg {Drivetrain.ToString().ToUpper()}]";

    public string Description
    {
        get => _description;

        private set
        {
            if (_description == value)
            {
                return;
            }

            _description = value;
            NotifyPropertyChanged();
        }
    }

    public int LockToLockRotation
    {
        get => _lockToLockRotation;

        internal set
        {
            if (_lockToLockRotation == value)
            {
                return;
            }

            _lockToLockRotation = value;
            NotifyPropertyChanged();
            UpdateDescription();
        }
    }
}
