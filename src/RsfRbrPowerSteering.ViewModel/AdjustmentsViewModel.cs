namespace RsfRbrPowerSteering.ViewModel;

public class AdjustmentsViewModel : NotifyPropertyChangedBase
{
    private const int AdjustmentDefault = 100;
    private readonly MainViewModel _mainViewModel;

    private int _weightRatio = 50;
    private int _fwd = AdjustmentDefault;
    private int _rwd = AdjustmentDefault;
    private int _awd = AdjustmentDefault;
    private int _gravel = AdjustmentDefault;
    private int _tarmac = AdjustmentDefault;
    private int _snow = AdjustmentDefault;
    private string? _validationMessageDefault;
    private string? _validationMessageWeight;
    private SurfaceKind? _primarySurface;

    internal AdjustmentsViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public int MinimumDefault { get; } = 10;
    public int MaximumDefault { get; } = 1000;

    public int MinimumRatio { get; } = 0;
    public int MaximumRatio { get; } = 100;

    public string RangeMessageDefault
        => _validationMessageDefault
            ?? (_validationMessageDefault = string.Format(ViewModelTexts.RangeMessageFormat, MinimumDefault, MaximumDefault));

    public string RangeMessageRatio
        => _validationMessageWeight
            ?? (_validationMessageWeight = string.Format(ViewModelTexts.RangeMessageFormat, MinimumRatio, MaximumRatio));

    public int WeightRatio
    {
        get => _weightRatio;
        set
        {
            RangeUtility.EnsureRange(ref value, MinimumRatio, MaximumRatio);

            if (_weightRatio == value)
            {
                return;
            }

            _weightRatio = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(LockToLockRotationRatio));
            _mainViewModel.ReCalculate();
        }
    }

    public int LockToLockRotationRatio
    {
        get => 100 - _weightRatio;
        set => WeightRatio = 100 - value;
    }

    private bool IsPrimarySurface(SurfaceKind? surface)
        => _primarySurface == surface;

    private void NotifySurfaceChanged(
        SurfaceKind? original,
        SurfaceKind? changed,
        SurfaceKind surfaceKind,
        string propertyNameSurface,
        string propertyNameSurfaceOrNull)
    {
        if (original == surfaceKind
            || changed == surfaceKind
            || !original.HasValue && changed != surfaceKind)
        {
            NotifyPropertyChanged(propertyNameSurface);
            NotifyPropertyChanged(propertyNameSurfaceOrNull);
            _mainViewModel.PrimaryTemplate.UpdateFfbSensesHaveValue();
            _mainViewModel.SecondaryTemplate.UpdateFfbSensesHaveValue();
        }
    }

    private void CopyFfbSensToOtherSurfaces()
    {
        _mainViewModel.PrimaryTemplate.CopyFfbSensToOtherSurfaces(_primarySurface);
        _mainViewModel.SecondaryTemplate.CopyFfbSensToOtherSurfaces(_primarySurface);
    }

    private void SetPrimarySurface(SurfaceKind? surface)
    {
        if (_primarySurface == surface)
        {
            return;
        }

        SurfaceKind? original = _primarySurface;
        _primarySurface = surface;

        if (original.HasValue && !surface.HasValue)
        {
            NotifyPropertyChanged(nameof(IsPrimarySurfaceNull));
            NotifyPropertyChanged(nameof(IsPrimarySurfaceSet));
            NotifyPropertyChanged(nameof(IsPrimarySurfaceGravelOrNull));
            NotifyPropertyChanged(nameof(IsPrimarySurfaceTarmacOrNull));
            NotifyPropertyChanged(nameof(IsPrimarySurfaceSnowOrNull));
            Gravel = 100;
            Tarmac = 100;
            Snow = 100;
        }
        else if (!original.HasValue && surface.HasValue)
        {
            NotifyPropertyChanged(nameof(IsPrimarySurfaceSet));
            NotifyPropertyChanged(nameof(IsPrimarySurfaceNull));
        }

        switch (surface)
        {
            case SurfaceKind.Gravel:
                Gravel = 100;
                break;

            case SurfaceKind.Tarmac:
                Tarmac = 100;
                break;

            case SurfaceKind.Snow:
                Snow = 100;
                break;
        }

        NotifySurfaceChanged(
            original,
            surface,
            SurfaceKind.Gravel,
            nameof(IsPrimarySurfaceGravel),
            nameof(IsPrimarySurfaceGravelOrNull));
        NotifySurfaceChanged(
            original,
            surface,
            SurfaceKind.Tarmac,
            nameof(IsPrimarySurfaceTarmac),
            nameof(IsPrimarySurfaceTarmacOrNull));
        NotifySurfaceChanged(
            original,
            surface,
            SurfaceKind.Snow,
            nameof(IsPrimarySurfaceSnow),
            nameof(IsPrimarySurfaceSnowOrNull));
        CopyFfbSensToOtherSurfaces();
    }

    public bool IsPrimarySurfaceGravel
    {
        get => IsPrimarySurface(SurfaceKind.Gravel);
        set => SetPrimarySurface(SurfaceKind.Gravel);
    }

    public bool IsPrimarySurfaceTarmac
    {
        get => IsPrimarySurface(SurfaceKind.Tarmac);
        set => SetPrimarySurface(SurfaceKind.Tarmac);
    }

    public bool IsPrimarySurfaceSnow
    {
        get => IsPrimarySurface(SurfaceKind.Snow);
        set => SetPrimarySurface(SurfaceKind.Snow);
    }

    public bool IsPrimarySurfaceNull
    {
        get => IsPrimarySurface(null);
        set => SetPrimarySurface(null);
    }

    public SurfaceKind? PrimarySurface
    {
        get => _primarySurface;
        internal set => SetPrimarySurface(value);
    }

    public bool IsPrimarySurfaceSet => !IsPrimarySurfaceNull;
    public bool IsPrimarySurfaceGravelOrNull => IsPrimarySurfaceNull || IsPrimarySurfaceGravel;
    public bool IsPrimarySurfaceTarmacOrNull => IsPrimarySurfaceNull || IsPrimarySurfaceTarmac;
    public bool IsPrimarySurfaceSnowOrNull => IsPrimarySurfaceNull || IsPrimarySurfaceSnow;

    public int Fwd
    {
        get => _fwd;
        set
        {
            RangeUtility.EnsureRange(ref value, MinimumDefault, MaximumDefault);

            if (_fwd == value)
            {
                return;
            }

            _fwd = value;
            NotifyPropertyChanged();
            _mainViewModel.ReCalculate();
        }
    }

    public int Rwd
    {
        get => _rwd;
        set
        {
            RangeUtility.EnsureRange(ref value, MinimumDefault, MaximumDefault);

            if (_rwd == value)
            {
                return;
            }

            _rwd = value;
            NotifyPropertyChanged();
            _mainViewModel.ReCalculate();
        }
    }

    public int Awd
    {
        get => _awd;
        set
        {
            RangeUtility.EnsureRange(ref value, MinimumDefault, MaximumDefault);

            if (_awd == value)
            {
                return;
            }

            _awd = value;
            NotifyPropertyChanged();
            _mainViewModel.ReCalculate();
        }
    }

    public int Gravel
    {
        get => _gravel;
        set
        {
            RangeUtility.EnsureRange(ref value, MinimumDefault, MaximumDefault);

            if (_gravel == value)
            {
                return;
            }

            _gravel = value;
            NotifyPropertyChanged();
            CopyFfbSensToOtherSurfaces();
            _mainViewModel.ReCalculate();
        }
    }


    public int Tarmac
    {
        get => _tarmac;
        set
        {
            RangeUtility.EnsureRange(ref value, MinimumDefault, MaximumDefault);

            if (_tarmac == value)
            {
                return;
            }

            _tarmac = value;
            NotifyPropertyChanged();
            CopyFfbSensToOtherSurfaces();
            _mainViewModel.ReCalculate();
        }
    }

    public int Snow
    {
        get => _snow;
        set
        {
            RangeUtility.EnsureRange(ref value, MinimumDefault, MaximumDefault);

            if (_snow == value)
            {
                return;
            }

            _snow = value;
            NotifyPropertyChanged();
            CopyFfbSensToOtherSurfaces();
            _mainViewModel.ReCalculate();
        }
    }

    internal void ResetToDefaults()
    {
        WeightRatio = 50;
        SetPrimarySurface(null);
        Fwd = AdjustmentDefault;
        Rwd = AdjustmentDefault;
        Awd = AdjustmentDefault;
        Gravel = AdjustmentDefault;
        Tarmac = AdjustmentDefault;
        Snow = AdjustmentDefault;
    }
}
