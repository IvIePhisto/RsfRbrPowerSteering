using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RsfRbrPowerSteering.ViewModel;

public class NotifyDataErrorBase : NotifyPropertyChangedBase, INotifyDataErrorInfo
{
    private readonly Dictionary<string, string> _propertyErrors = new();

    public bool HasErrors => _propertyErrors.Count > 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName != null
            && _propertyErrors.TryGetValue(propertyName, out string? error))
        {
            yield return error;
        }
    }

    internal void SetError(string error, [CallerMemberName] string? propertyName = null)
    {
        if (string.IsNullOrEmpty(error))
        {
            throw new ArgumentNullException(nameof(error));
        }

        if (propertyName == null)
        {
            return;
        }

        if (_propertyErrors.TryGetValue(propertyName, out string? currentError)
            && currentError == error)
        {
            return;
        }

        _propertyErrors[propertyName] = error;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    internal void ClearError([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null)
        {
            return;
        }

        _propertyErrors.Remove(propertyName);
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
