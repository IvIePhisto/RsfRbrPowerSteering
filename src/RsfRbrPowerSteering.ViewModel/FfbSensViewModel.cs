using RsfRbrPowerSteering.Model.Rsf;

namespace RsfRbrPowerSteering.ViewModel;

public class FfbSensViewModel : NotifyPropertyChangedBase
{
    private int? _gravel;
    private int? _tarmac;
    private int? _snow;

    public int? Gravel
    {
        get => _gravel;

        internal set
        {
            if (_gravel == value)
            {
                return;
            }

            _gravel = value;
            NotifyPropertyChanged();
        }
    }

    public int? Tarmac
    {
        get => _tarmac;

        internal set
        {
            if (_tarmac == value)
            {
                return;
            }

            _tarmac = value;
            NotifyPropertyChanged();
        }
    }

    public int? Snow
    {
        get => _snow;

        internal set
        {
            if (_snow == value)
            {
                return;
            }

            _snow = value;
            NotifyPropertyChanged();
        }
    }

    internal void ApplyFfbSens(PersonalCarFfbSens ffbSens)
    {
        Gravel = ffbSens.Gravel;
        Tarmac = ffbSens.Tarmac;
        Snow = ffbSens.Snow;
    }
}
