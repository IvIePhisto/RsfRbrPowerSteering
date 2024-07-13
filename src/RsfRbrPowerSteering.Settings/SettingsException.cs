namespace RsfRbrPowerSteering.Settings;

public class SettingsException : Exception
{
    internal SettingsException(string message, Exception e) : base(message, e) { }
}
