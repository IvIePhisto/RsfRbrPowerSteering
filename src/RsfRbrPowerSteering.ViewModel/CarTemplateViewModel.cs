using RsfRbrPowerSteering.Model;
using RsfRbrPowerSteering.Model.Calculation;
using RsfRbrPowerSteering.Model.Rsf;
using RsfRbrPowerSteering.Settings;

namespace RsfRbrPowerSteering.ViewModel;

public class CarTemplateViewModel : NotifyDataErrorBase
{
    private const int FfbSensDefault = 300;

    private readonly MainViewModel _mainViewModel;
    private double _lockToLockRotationForSlider;
    private Drivetrain _drivetrain = Drivetrain.Fwd;
    private int? _ffbSensGravel;
    private int? _ffbSensTarmac;
    private int? _ffbSensSnow;
    private int _lockToLockRotationForComboBox;
    private bool _isDrivetrainFwd = true;
    private bool _isDrivetrainRwd;
    private bool _isDrivetrainAwd;
    private int _selectedCarId;
    private int _weightKg;
    private bool _ffbSensesHaveValue;

    internal CarTemplateViewModel(MainViewModel mainWindow)
    {
        _mainViewModel = mainWindow;
        _selectedCarId = _mainViewModel.DefaultCar.Id;
        ResetToDefaults(0, 0);
    }

    private void ResetSelectedCarId()
    {
        if (_selectedCarId == _mainViewModel.DefaultCar.Id)
        {
            return;
        }

        _selectedCarId = _mainViewModel.DefaultCar.Id;
        NotifyPropertyChanged(nameof(SelectedCarId));
    }

    public int SelectedCarId
    {
        get => _selectedCarId;
        set
        {
            if (_selectedCarId == value)
            {
                return;
            }

            _selectedCarId = value;
            NotifyPropertyChanged();

            if (value == _mainViewModel.DefaultCar.Id)
            {
                return;
            }

            CarViewModel car = _mainViewModel.CarsById[value];
            _weightKg = car.WeightKg;
            NotifyPropertyChanged(nameof(WeightKg));

            LockToLockRotationViewModel lockToLockRotation = _mainViewModel.LockToLockRotationsByIntValue[car.LockToLockRotation];

            if (lockToLockRotation.IntValue != _lockToLockRotationForComboBox)
            {
                _lockToLockRotationForComboBox = lockToLockRotation.IntValue;
                NotifyPropertyChanged(nameof(LockToLockRotation));
            }

            if (lockToLockRotation.DoubleValue != _lockToLockRotationForSlider)
            {
                _lockToLockRotationForSlider = lockToLockRotation.DoubleValue;
                NotifyPropertyChanged(nameof(LockToLockRotationForSlider));
            }

            if (car.Drivetrain != _drivetrain)
            {
                _drivetrain = car.Drivetrain;

                if (car.Drivetrain == Drivetrain.Fwd && !_isDrivetrainFwd)
                {
                    _isDrivetrainFwd = true;
                    NotifyPropertyChanged(nameof(IsDrivetrainFwd));
                }
                else if (_isDrivetrainFwd)
                {
                    _isDrivetrainFwd = false;
                    NotifyPropertyChanged(nameof(IsDrivetrainFwd));
                }

                if (car.Drivetrain == Drivetrain.Rwd && !_isDrivetrainRwd)
                {
                    _isDrivetrainRwd = true;
                    NotifyPropertyChanged(nameof(IsDrivetrainRwd));
                }
                else if (_isDrivetrainRwd)
                {
                    _isDrivetrainRwd = false;
                    NotifyPropertyChanged(nameof(IsDrivetrainRwd));
                }

                if (car.Drivetrain == Drivetrain.Awd && !_isDrivetrainAwd)
                {
                    _isDrivetrainAwd = true;
                    NotifyPropertyChanged(nameof(IsDrivetrainAwd));
                }
                else if (_isDrivetrainAwd)
                {
                    _isDrivetrainAwd = false;
                    NotifyPropertyChanged(nameof(IsDrivetrainAwd));
                }
            }

            if (car.FfbSensPersonal.Gravel != _ffbSensGravel)
            {
                _ffbSensGravel = car.FfbSensPersonal.Gravel;
                NotifyPropertyChanged(nameof(FfbSensGravel));
            }

            if (car.FfbSensPersonal.Tarmac != _ffbSensTarmac)
            {
                _ffbSensTarmac = car.FfbSensPersonal.Tarmac;
                NotifyPropertyChanged(nameof(FfbSensTarmac));
            }

            if (car.FfbSensPersonal.Snow != _ffbSensSnow)
            {
                _ffbSensSnow = car.FfbSensPersonal.Snow;
                NotifyPropertyChanged(nameof(FfbSensSnow));
            }

            UpdateFfbSensesHaveValue();
            CopyFfbSensToOtherSurfaces(_mainViewModel.Adjustments.PrimarySurface);
        }
    }

    public int WeightKg
    {
        get => _weightKg;

        set
        {
            if (_weightKg == value)
            {
                return;
            }

            _weightKg = value;
            NotifyPropertyChanged();
            ResetSelectedCarId();
            _mainViewModel.ReCalculate();
        }
    }

    public double LockToLockRotationForSlider
    {
        get => _lockToLockRotationForSlider;
        set
        {
            RangeUtility.EnsureRange(ref value, _mainViewModel.LockToLockRotationMinimumForSlider, _mainViewModel.LockToLockRotationMaximumForSlider);

            if (value == _lockToLockRotationForSlider)
            {
                return;
            }

            _lockToLockRotationForSlider = value;
            NotifyPropertyChanged();
            ResetSelectedCarId();

            LockToLockRotationViewModel lockToLockRotation = _mainViewModel.LockToLockRotations.Single(i => i.DoubleValue == value);

            if (lockToLockRotation.IntValue != _lockToLockRotationForComboBox)
            {
                _lockToLockRotationForComboBox = lockToLockRotation.IntValue;
                NotifyPropertyChanged(nameof(LockToLockRotation));
            }

            _mainViewModel.ReCalculate();
        }
    }

    public int LockToLockRotation
    {
        get => _lockToLockRotationForComboBox;
        set
        {
            RangeUtility.EnsureRange(ref value, _mainViewModel.LockToLockRotationMinimumForComboBox, _mainViewModel.LockToLockRotationMaximumForComboBox);

            if (value == _lockToLockRotationForComboBox)
            {
                return;
            }

            _lockToLockRotationForComboBox = value;
            NotifyPropertyChanged();
            ResetSelectedCarId();
            LockToLockRotationViewModel lockToLockRotation = _mainViewModel.LockToLockRotations.Single(i => i.IntValue == value);
            double valueForSlider = lockToLockRotation.DoubleValue;

            if (valueForSlider != _lockToLockRotationForSlider)
            {
                _lockToLockRotationForSlider = valueForSlider;
                NotifyPropertyChanged(nameof(LockToLockRotationForSlider));
            }
        }
    }

    public bool IsDrivetrainFwd
    {
        get => _isDrivetrainFwd;
        set
        {
            if (value == _isDrivetrainFwd)
            {
                return;
            }

            _isDrivetrainFwd = value;
            NotifyPropertyChanged();

            if (!value)
            {
                return;
            }

            if (_isDrivetrainRwd)
            {
                _isDrivetrainRwd = false;
                NotifyPropertyChanged(nameof(IsDrivetrainRwd));
            }

            if (_isDrivetrainAwd)
            {
                _isDrivetrainAwd = false;
                NotifyPropertyChanged(nameof(IsDrivetrainAwd));
            }

            _drivetrain = Drivetrain.Fwd;
            ResetSelectedCarId();
        }
    }

    public bool IsDrivetrainRwd
    {
        get => _isDrivetrainRwd;
        set
        {
            if (value == _isDrivetrainRwd)
            {
                return;
            }

            _isDrivetrainRwd = value;
            NotifyPropertyChanged();

            if (!value)
            {
                return;
            }

            if (_isDrivetrainFwd)
            {
                _isDrivetrainFwd = false;
                NotifyPropertyChanged(nameof(IsDrivetrainFwd));
            }

            if (_isDrivetrainAwd)
            {
                _isDrivetrainAwd = false;
                NotifyPropertyChanged(nameof(IsDrivetrainAwd));
            }

            _drivetrain = Drivetrain.Rwd;
            ResetSelectedCarId();
        }
    }

    public bool IsDrivetrainAwd
    {
        get => _isDrivetrainAwd;
        set
        {
            if (value == _isDrivetrainAwd)
            {
                return;
            }

            _isDrivetrainAwd = value;
            NotifyPropertyChanged();

            if (!value)
            {
                return;
            }

            if (_isDrivetrainFwd)
            {
                _isDrivetrainFwd = false;
                NotifyPropertyChanged(nameof(IsDrivetrainFwd));
            }

            if (_isDrivetrainRwd)
            {
                _isDrivetrainRwd = false;
                NotifyPropertyChanged(nameof(IsDrivetrainRwd));
            }

            _drivetrain = Drivetrain.Awd;
            ResetSelectedCarId();
        }
    }

    private static int? GetOtherFfbSens(int? ffbSens, int factorCurrent, int factorOther)
        => ffbSens.HasValue
            ? Convert.ToInt32(ffbSens / (factorCurrent / 100M) * factorOther / 100M)
            : null;

    private void SetFfbSensGravel(int? value)
    {
        RangeUtility.EnsureRange(ref value, _mainViewModel.FfbSensMinimum, _mainViewModel.FfbSensMaximum);

        if (value == _ffbSensGravel)
        {
            return;
        }

        _ffbSensGravel = value;
        NotifyPropertyChanged(nameof(FfbSensGravel));
        UpdateFfbSensesHaveValue();
    }

    public int? FfbSensGravel
    {
        get => _ffbSensGravel;
        set
        {
            if (!_mainViewModel.Adjustments.IsPrimarySurfaceNull
                && !_mainViewModel.Adjustments.IsPrimarySurfaceGravel)
            {
                return;
            }

            SetFfbSensGravel(value);

            if (_mainViewModel.Adjustments.IsPrimarySurfaceGravel)
            {
                CopyFfbSensToOtherSurfaces(SurfaceKind.Gravel);
            }

            _mainViewModel.ReCalculate();
        }
    }

    private void SetFfbSensTarmac(int? value)
    {
        RangeUtility.EnsureRange(ref value, _mainViewModel.FfbSensMinimum, _mainViewModel.FfbSensMaximum);

        if (value == _ffbSensTarmac)
        {
            return;
        }

        _ffbSensTarmac = value;
        NotifyPropertyChanged(nameof(FfbSensTarmac));
        UpdateFfbSensesHaveValue();
    }

    public int? FfbSensTarmac
    {
        get => _ffbSensTarmac;
        set
        {
            if (!_mainViewModel.Adjustments.IsPrimarySurfaceNull
                && !_mainViewModel.Adjustments.IsPrimarySurfaceTarmac)
            {
                return;
            }

            SetFfbSensTarmac(value);

            if (_mainViewModel.Adjustments.IsPrimarySurfaceTarmac)
            {
                CopyFfbSensToOtherSurfaces(SurfaceKind.Tarmac);
            }

            _mainViewModel.ReCalculate();
        }
    }

    private void SetFfbSensSnow(int? value)
    {
        RangeUtility.EnsureRange(ref value, _mainViewModel.FfbSensMinimum, _mainViewModel.FfbSensMaximum);

        if (value == _ffbSensSnow)
        {
            return;
        }

        _ffbSensSnow = value;
        NotifyPropertyChanged(nameof(FfbSensSnow));
        UpdateFfbSensesHaveValue();
    }

    public int? FfbSensSnow
    {
        get => _ffbSensSnow;
        set
        {
            if (!_mainViewModel.Adjustments.IsPrimarySurfaceNull
                && !_mainViewModel.Adjustments.IsPrimarySurfaceSnow)
            {
                return;
            }

            SetFfbSensSnow(value);

            if (_mainViewModel.Adjustments.IsPrimarySurfaceSnow)
            {
                CopyFfbSensToOtherSurfaces(SurfaceKind.Snow);
            }

            _mainViewModel.ReCalculate();
        }
    }

    internal void UpdateFfbSensesHaveValue()
    {
        FfbSensesHaveValue =
            FfbSensGravel.HasValue
            && FfbSensTarmac.HasValue
            && FfbSensSnow.HasValue;

        void UpdateError(bool isPrimaryOrNull, int? value, string propertyName)
        {
            if (isPrimaryOrNull && !value.HasValue)
            {
                SetError(ViewModelTexts.ValueMissingError, propertyName);
            }
            else
            {
                ClearError(propertyName);
            }
        }

        UpdateError(
            _mainViewModel.Adjustments.IsPrimarySurfaceGravelOrNull,
            FfbSensGravel,
            nameof(FfbSensGravel));
        UpdateError(
            _mainViewModel.Adjustments.IsPrimarySurfaceTarmacOrNull,
            FfbSensTarmac,
            nameof(FfbSensTarmac));
        UpdateError(
            _mainViewModel.Adjustments.IsPrimarySurfaceSnowOrNull,
            FfbSensSnow,
            nameof(FfbSensSnow));
    }

    public bool FfbSensesHaveValue
    {
        get => _ffbSensesHaveValue;

        private set
        {
            if (_ffbSensesHaveValue == value)
            {
                return;
            }

            _ffbSensesHaveValue = value;
            NotifyPropertyChanged();
        }
    }

    internal void ApplyFfbSens(PersonalCarFfbSens ffbSens)
    {
        FfbSensGravel = ffbSens.Gravel;
        FfbSensTarmac = ffbSens.Tarmac;
        FfbSensSnow = ffbSens.Snow;
    }

    public CalculationCar ToCalculationCar()
        => new CalculationCar(
            LockToLockRotation,
            WeightKg,
            new CarFfbSens
            {
                Gravel = FfbSensGravel,
                Tarmac = FfbSensTarmac,
                Snow = FfbSensSnow
            },
            _drivetrain);

    public void ApplySettings(CarSettings settings)
    {
        SelectedCarId = settings.SelectedCarId
            ?? _mainViewModel.DefaultCar.Id;

        switch (settings.Drivetrain)
        {
            case Drivetrain.Fwd:
                IsDrivetrainFwd = true;
                break;

            case Drivetrain.Rwd:
                IsDrivetrainRwd = true;
                break;

            case Drivetrain.Awd:
                IsDrivetrainAwd = true;
                break;

            default:
                throw new InvalidCastException($"Unexpected {typeof(Drivetrain)} value: {settings.Drivetrain}");
        }

        WeightKg = settings.WeightKg;
        LockToLockRotation = settings.LockToLockRotation;
        FfbSensGravel = settings.FfbSensGravel;
        FfbSensTarmac = settings.FfbSensTarmac;
        FfbSensSnow = settings.FfbSensSnow;
    }

    internal CarSettings ToSettings()
        => new CarSettings
            {
                SelectedCarId = SelectedCarId,
                Drivetrain = _drivetrain,
                WeightKg = WeightKg,
                LockToLockRotation = LockToLockRotation,
                FfbSensGravel = FfbSensGravel,
                FfbSensTarmac = FfbSensTarmac,
                FfbSensSnow = FfbSensSnow
            };

    internal void ResetToDefaults(int lockToLockRotation, int weightKg)
    {
        SelectedCarId = 0;
        LockToLockRotation = lockToLockRotation;
        WeightKg = weightKg;
        IsDrivetrainFwd = true;
        FfbSensGravel = FfbSensDefault;
        FfbSensTarmac = FfbSensDefault;
        FfbSensSnow = FfbSensDefault;
    }

    internal void CopyFfbSensToOtherSurfaces(SurfaceKind? surface)
    {
        if (surface == null)
        {
            return;
        }

        int? value;
        int adjustmentPrimary;
        SurfaceKind[] otherSurfaces;

        switch (surface)
        {
            case SurfaceKind.Gravel:
                value = _ffbSensGravel;
                adjustmentPrimary = _mainViewModel.Adjustments.Gravel;
                otherSurfaces = [SurfaceKind.Tarmac, SurfaceKind.Snow];
                break;

            case SurfaceKind.Tarmac:
                value = _ffbSensTarmac;
                adjustmentPrimary = _mainViewModel.Adjustments.Tarmac;
                otherSurfaces = [SurfaceKind.Gravel, SurfaceKind.Snow];
                break;

            case SurfaceKind.Snow:
                value = _ffbSensSnow;
                adjustmentPrimary = _mainViewModel.Adjustments.Snow;
                otherSurfaces = [SurfaceKind.Gravel, SurfaceKind.Tarmac];
                break;

            default:
                throw new InvalidOperationException($"Unexpected {typeof(SurfaceKind)} value.");
        }

        foreach (SurfaceKind otherSurface in otherSurfaces)
        {
            int adjustmentOther;
            Action<int?> setOtherFfbSens;

            switch (otherSurface)
            {
                case SurfaceKind.Gravel:
                    adjustmentOther = _mainViewModel.Adjustments.Gravel;
                    setOtherFfbSens = SetFfbSensGravel;
                    break;

                case SurfaceKind.Tarmac:
                    adjustmentOther = _mainViewModel.Adjustments.Tarmac;
                    setOtherFfbSens = SetFfbSensTarmac;
                    break;

                case SurfaceKind.Snow:
                    adjustmentOther = _mainViewModel.Adjustments.Snow;
                    setOtherFfbSens = SetFfbSensSnow;
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected {typeof(SurfaceKind)} value: {otherSurface}");
            }

            int? otherFfbSens = GetOtherFfbSens(
                value,
                adjustmentPrimary,
                adjustmentOther);
            setOtherFfbSens(otherFfbSens);
        }
    }
}
