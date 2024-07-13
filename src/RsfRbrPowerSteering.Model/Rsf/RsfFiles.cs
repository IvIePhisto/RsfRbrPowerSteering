using RsfRbrPowerSteering.Model;

namespace RsfRbrPowerSteering.Model.Rsf;

public static class RsfFiles
{
    public static FileInfo CarsFile { get; } = new FileInfo(Path.Combine("rsfdata", "cache", "cars.json"));
    public static FileInfo CarsDataFile { get; } = new FileInfo(Path.Combine("rsfdata", "cache", "cars_data.json"));
    public static FileInfo PersonalRsfIniFile { get; } = new FileInfo("rallysimfans_personal.ini");

    public static bool Check(out IReadOnlyList<string> missingFilePaths)
    {
        var errorMessagesLocal = new List<string>();
        bool isValid = true;

        if (!PersonalRsfIniFile.Exists)
        {
            errorMessagesLocal.Add(PersonalRsfIniFile.ToString());
            isValid = false;
        }

        if (!CarsFile.Exists)
        {
            errorMessagesLocal.Add(CarsFile.ToString());
            isValid = false;
        }

        if (!CarsDataFile.Exists)
        {
            errorMessagesLocal.Add(CarsDataFile.ToString());
            isValid = false;
        }

        missingFilePaths = errorMessagesLocal;

        return isValid;
    }
}
