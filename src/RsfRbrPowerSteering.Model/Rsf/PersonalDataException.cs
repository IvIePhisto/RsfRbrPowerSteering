namespace RsfRbrPowerSteering.Model.Rsf;

public class PersonalDataException : Exception
{
    public PersonalDataException(string message) : base(message) { }

    internal PersonalDataException(string message, Exception innerException) : base(message, innerException) { }
}
