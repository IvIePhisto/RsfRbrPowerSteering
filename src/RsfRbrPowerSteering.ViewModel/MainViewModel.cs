using RsfRbrPowerSteering.Model;
using RsfRbrPowerSteering.Model.Calculation;
using RsfRbrPowerSteering.Model.Rsf;
using RsfRbrPowerSteering.Settings;
using RsfRbrPowerSteering.ViewModel.Commands;
using RsfRbrPowerSteering.ViewModel.Interfaces;
using System.Collections.ObjectModel;
using System.Reflection;

namespace RsfRbrPowerSteering.ViewModel;

public class MainViewModel : NotifyPropertyChangedBase
{
    private readonly PersonalData _personalData = new PersonalData();
    IReadOnlyDictionary<int, CarInfo>? _carInfos;
    private int _lockToLockRotationMinimum = 0;
    private int _lockToLockRotationMaximum = 0;
    private bool _isExclusiveCommandRunning;
    private bool _isInformationVisible = true;
    private bool _isReCalcuationEnabled = false;

    public MainViewModel(
        ICommandManager commandManager,
        IMessageService messageService)
    {
        Commands = new ViewModelCommands(
            commandManager,
            messageService,
            this);
        Cars.Add(DefaultCar);
        Adjustments = new AdjustmentsViewModel(this);
        PrimaryTemplate = new CarTemplateViewModel(this);
        SecondaryTemplate = new CarTemplateViewModel(this);
        FfbSensRangeMessage = string.Format(ViewModelTexts.RangeMessageFormat, FfbSensMinimum, FfbSensMaximum);
        Version? version = Assembly.GetEntryAssembly()?.GetName()?.Version;
        VersionText = string.Format(
            ViewModelTexts.VersionTextFormat,
            version?.Major,
            version?.Minor,
            version?.Build);
    }

    public event Action? LockToLockRotationsChanged;

    public string VersionText { get; }
    public int FfbSensMinimum { get; } = 10;
    public int FfbSensMaximum { get; } = 5000;
    public string FfbSensRangeMessage { get; }
    public ViewModelCommands Commands { get; }

    public bool IsDescriptionVisible
    {
        get => _isInformationVisible;

        private set
        {
            _isInformationVisible = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(IsDescriptionHidden));
        }
    }

    public bool IsDescriptionHidden => !_isInformationVisible;

    public CarViewModel DefaultCar { get; } = new CarViewModel();

    public int LockToLockRotationMinimumForComboBox
    {
        get => _lockToLockRotationMinimum;
        set
        {
            if (_lockToLockRotationMinimum == value)
            {
                return;
            }

            _lockToLockRotationMinimum = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(LockToLockRotationMinimumForSlider));
        }
    }

    public int LockToLockRotationMaximumForComboBox
    {
        get => _lockToLockRotationMaximum;
        set
        {
            if (_lockToLockRotationMaximum == value)
            {
                return;
            }

            _lockToLockRotationMaximum = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(LockToLockRotationMaximumForSlider));
        }
    }

    public double LockToLockRotationMinimumForSlider => _lockToLockRotationMinimum;

    public double LockToLockRotationMaximumForSlider => _lockToLockRotationMaximum;

    public bool IsExclusiveCommandRunning
    {
        get => _isExclusiveCommandRunning;

        set
        {
            if (_isExclusiveCommandRunning == value)
            {
                return;
            }

            _isExclusiveCommandRunning = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(IsNoExclusiveCommandRunning));
        }
    }

    public bool IsNoExclusiveCommandRunning => !_isExclusiveCommandRunning;

    private readonly Dictionary<int, CarViewModel> _carsById = new Dictionary<int, CarViewModel>();
    internal IReadOnlyDictionary<int, CarViewModel> CarsById => _carsById;

    private readonly Dictionary<int, LockToLockRotationViewModel> _lockToLockRotationsByIntValue = new Dictionary<int, LockToLockRotationViewModel>();
    internal IReadOnlyDictionary<int, LockToLockRotationViewModel> LockToLockRotationsByIntValue => _lockToLockRotationsByIntValue;

    public ObservableCollection<CarViewModel> Cars { get; } = new ObservableCollection<CarViewModel>();
    public ObservableCollection<LockToLockRotationViewModel> LockToLockRotations { get; } = new ObservableCollection<LockToLockRotationViewModel>();
    public AdjustmentsViewModel Adjustments { get; }
    public CarTemplateViewModel PrimaryTemplate { get; }
    public CarTemplateViewModel SecondaryTemplate { get; }

    internal void ToggleDescriptionVisibility()
        => IsDescriptionVisible = !IsDescriptionVisible;

    internal async Task LoadCarsAsync(bool reCalculate = true)
    {
        _isReCalcuationEnabled = false;
        _carInfos = await CarInfo.ReadCarsAsync();
        _personalData.ReadFile();
        var carViewModels = Cars.ToDictionary(car => car.Id);
        var carIds = new HashSet<int>();
        var lockToLockRotations = new HashSet<int>();

        foreach ((int carId, PersonalCarFfbSens ffbSens, int? personalLockToLockRotation) in _personalData.GetCars())
        {
            if (!_carInfos.TryGetValue(carId, out CarInfo car))
            {
                // Unknown car.
                continue;
            }

            if (!carViewModels.TryGetValue(carId, out CarViewModel? carViewModel))
            {
                // Add new car:
                carViewModel = new CarViewModel(car);

                if (personalLockToLockRotation.HasValue)
                {
                    carViewModel.LockToLockRotation = personalLockToLockRotation.Value;
                }

                _carsById[carId] = carViewModel;
                Cars.Add(carViewModel);
            }

            lockToLockRotations.Add(car.LockToLockRotation);

            // Set sensitivities:
            carViewModel.FfbSensPersonal.ApplyFfbSens(ffbSens);

            if (PrimaryTemplate.SelectedCarId == carId)
            {
                PrimaryTemplate.ApplyFfbSens(ffbSens);
            }

            if (SecondaryTemplate.SelectedCarId == carId)
            {
                SecondaryTemplate.ApplyFfbSens(ffbSens);
            }

            carIds.Add(carId);
        }

        // Remove cars no longer available:
        for (int i = Cars.Count - 1; i >= 0; i--)
        {
            CarViewModel carViewModel = Cars[i];

            if (carViewModel.Id != DefaultCar.Id && !carIds.Contains(carViewModel.Id))
            {
                _carsById.Remove(carViewModel.Id);
                Cars.RemoveAt(i);
            }
        }

        LockToLockRotationMinimumForComboBox = lockToLockRotations.Min();
        LockToLockRotationMaximumForComboBox = lockToLockRotations.Max();
        bool wereLockToLockRotationsChanged = false;

        // Remove lock-to-lock rotations no longer available:
        for (int i = LockToLockRotations.Count - 1; i >= 0; i--)
        {
            LockToLockRotationViewModel lockToLockRotation = LockToLockRotations[i];

            if (!lockToLockRotations.Contains(lockToLockRotation.IntValue))
            {
                _lockToLockRotationsByIntValue.Remove(lockToLockRotation.IntValue);
                LockToLockRotations.RemoveAt(i);
                wereLockToLockRotationsChanged = true;
            }
        }

        // Add new lock-to-lock rotations:
        int j = 0;

        foreach (int lockToLockRotation in lockToLockRotations
            .Except(LockToLockRotations.Select(l => l.IntValue))
            .Order())
        {
            var newLockToLockRotation = new LockToLockRotationViewModel(lockToLockRotation);
            _lockToLockRotationsByIntValue[lockToLockRotation] = newLockToLockRotation;

            if (j < LockToLockRotations.Count)
            {
                LockToLockRotationViewModel existingLockToLockRotation;

                do
                {
                    existingLockToLockRotation = LockToLockRotations[j];
                    j++;
                }
                while (lockToLockRotation < existingLockToLockRotation.IntValue && j < LockToLockRotations.Count);
            }

            LockToLockRotations.Insert(j, new LockToLockRotationViewModel(lockToLockRotation));
            wereLockToLockRotationsChanged = true;
            j++;
        }

        if (wereLockToLockRotationsChanged)
        {
            LockToLockRotationsChanged?.Invoke();
        }

        _isReCalcuationEnabled = true;

        if (reCalculate)
        {
            ReCalculate();
        }
    }

    private IReadOnlyDictionary<int, CarFfbSens> CalculateFfbSenses()
    {
        if (!_isReCalcuationEnabled || _carInfos == null)
        {
            return new Dictionary<int, CarFfbSens>();
        }

        return CalculationUtility.CalculateFfbSenses(
            _carInfos.Values,
            Adjustments.WeightRatio / 100M,
            new DrivetrainFactors(Adjustments.Fwd / 100M, Adjustments.Rwd / 100M, Adjustments.Awd / 100M),
            PrimaryTemplate.ToCalculationCar(),
            SecondaryTemplate.ToCalculationCar());
    }

    internal void ReCalculate()
    {
        if (!_isReCalcuationEnabled)
        {
            return;
        }

        IReadOnlyDictionary<int, CarFfbSens> ffbSenses = CalculateFfbSenses();

        foreach ((int id, CarFfbSens ffbSens) in ffbSenses)
        {
            if (_carsById.TryGetValue(id, out CarViewModel? car))
            {
                car.FfbSensCalculated.ApplyFfbSens(ffbSens.ToPersonal());
            }
        }
    }

    internal async Task LoadSettingsAsync()
    {
        RootSettings? settings = await RootSettings.LoadAsync();

        if (settings != null)
        {
            _isReCalcuationEnabled = false;
            IsDescriptionVisible = settings.IsDescriptionVisible;
            Adjustments.PrimarySurface = string.IsNullOrEmpty(settings.PrimarySurface)
                ? null
                : Enum.Parse<SurfaceKind>(settings.PrimarySurface);
            Adjustments.WeightRatio = settings.AdjustmentWeightRatio;
            Adjustments.Gravel = settings.AdjustmentGravel;
            Adjustments.Tarmac = settings.AdjustmentTarmac;
            Adjustments.Snow = settings.AdjustmentSnow;
            Adjustments.Fwd = settings.AdjustmentFwd;
            Adjustments.Rwd = settings.AdjustmentRwd;
            Adjustments.Awd = settings.AdjustmentAwd;
            PrimaryTemplate.ApplySettings(settings.PrimaryCar);
            SecondaryTemplate.ApplySettings(settings.SecondaryCar);
            _isReCalcuationEnabled = true;
            ReCalculate();
        }
    }

    internal async Task SaveSettingsAsync()
    {
        await new RootSettings
            {
                IsDescriptionVisible = IsDescriptionVisible,
                PrimaryCar = PrimaryTemplate.ToSettings(),
                SecondaryCar = SecondaryTemplate.ToSettings(),
                PrimarySurface = Adjustments.PrimarySurface?.ToString() ?? string.Empty,
                AdjustmentWeightRatio = Adjustments.WeightRatio,
                AdjustmentGravel = Adjustments.Gravel,
                AdjustmentTarmac = Adjustments.Tarmac,
                AdjustmentSnow = Adjustments.Snow,
                AdjustmentFwd = Adjustments.Fwd,
                AdjustmentRwd = Adjustments.Rwd,
                AdjustmentAwd = Adjustments.Awd
            }
        .SaveAsync();
    }

    internal async Task ExportCarsAsync(FileInfo exportFile)
        => await _personalData.ExportCarsAsync(exportFile);

    internal async Task ImportCarsAsync(FileInfo importFile)
    {
        await _personalData.ImportCarsAsync(importFile);
        await LoadCarsAsync();
    }

    internal async Task ApplyScalingAsync()
    {
        int primaryCarId = PrimaryTemplate.SelectedCarId;
        int secondaryCarId = SecondaryTemplate.SelectedCarId;
        _carInfos = await CarInfo.ReadCarsAsync();
        IReadOnlyDictionary<int, CarFfbSens> ffbSenses = CalculateFfbSenses();
        _personalData.ReadFile();
        _personalData.ApplyFfbSens(ffbSenses.Select(kvp => (kvp.Key, kvp.Value.ToPersonal())));
        _personalData.WriteFile();
        await LoadCarsAsync();
        PrimaryTemplate.SelectedCarId = primaryCarId;
        SecondaryTemplate.SelectedCarId = secondaryCarId;
    }

    internal async Task ClearFfbSensAsync()
    {
        _personalData.ReadFile();
        _personalData.ApplyFfbSens(_personalData.GetCars().Select((i => (i.CarId, new PersonalCarFfbSens()))));
        _personalData.WriteFile();
        await LoadCarsAsync();
    }

    internal void ResetToDefaults()
    {
        _isReCalcuationEnabled = false;
        int[]? carWeightsKg = _carInfos?.Values.Select(c => c.WeightKg).Distinct().Order().ToArray();
        IReadOnlyList<int> weightsKg = carWeightsKg == null || carWeightsKg.Length == 0
            ? [500, 1000]
            : carWeightsKg;
        Adjustments.ResetToDefaults();
        PrimaryTemplate.ResetToDefaults(LockToLockRotations.FirstOrDefault()?.IntValue ?? 0, weightsKg.First());
        SecondaryTemplate.ResetToDefaults(LockToLockRotations.LastOrDefault()?.IntValue ?? 0, weightsKg.Last());
        _isReCalcuationEnabled = true;
        ReCalculate();
    }
}
