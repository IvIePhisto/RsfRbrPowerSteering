using IniParser.Model;
using IniParser;
using System.Text;
using System.Text.Json;

namespace RsfRbrPowerSteering.Model.Rsf;

public class PersonalData
{
    private static readonly Encoding IniEncoding = Encoding.UTF8;
    private static readonly FileIniDataParser parser;

    static PersonalData()
    {
        parser = new FileIniDataParser();
        parser.Parser.Configuration.AssigmentSpacer = string.Empty;
    }

    private readonly FileInfo _personalRsfIniFile;
    private IniData? _personalIniData;

    public PersonalData()
    {
        _personalRsfIniFile = RsfFiles.PersonalRsfIniFile;
    }

    public bool WasLoaded => _personalIniData != null;

    private SectionDataCollection Sections
        => _personalIniData?.Sections
            ?? throw new InvalidOperationException(ModelTexts.PersonalDataNotLoadedError);

    public void ReadFile()
        => _personalIniData = parser.ReadFile(_personalRsfIniFile.FullName, IniEncoding);

    public void WriteFile()
        => parser.WriteFile(_personalRsfIniFile.FullName, _personalIniData, IniEncoding);

    public IEnumerable<(int CarId, PersonalCarFfbSens FfbSens, int? LockToLockRotation)> GetCars()
    {
        if (_personalIniData == null)
        {
            throw new InvalidOperationException(ModelTexts.PersonalDataNotLoadedError);
        }

        foreach (SectionData sectionData in Sections)
        {
            if (sectionData.SectionName.StartsWith("car")
                && int.TryParse(sectionData.SectionName[3..], out int carId))
            {
                KeyDataCollection section = _personalIniData[$"car{carId}"];
                PersonalCarFfbSens carFfbSens;
                int? lockToLockRotation;

                try
                {
                    int? GetFfbSens(string sectionName)
                    {
                        string ffbSensRaw = section[sectionName];

                        if (string.IsNullOrEmpty(ffbSensRaw))
                        {
                            return null;
                        }

                        return Convert.ToInt32(ffbSensRaw);
                    }

                    carFfbSens = new PersonalCarFfbSens
                    {
                        Gravel = GetFfbSens("forcefeedbacksensitivitygravel"),
                        Tarmac = GetFfbSens("forcefeedbacksensitivitytarmac"),
                        Snow = GetFfbSens("forcefeedbacksensitivitysnow")
                    };
                    string lockToLockRotationRaw = section["steeringrotation"];
                    lockToLockRotation = string.IsNullOrEmpty(lockToLockRotationRaw)
                        ? null
                        : Convert.ToInt32(lockToLockRotationRaw);
                }
                catch (Exception e)
                {
                    throw new PersonalDataException(
                        string.Format(
                            ModelTexts.PersonalDataReadCarErrorExceptionFormat,
                            carId,
                            e.Message),
                        e);
                }

                yield return (
                    carId,
                    carFfbSens,
                    lockToLockRotation);
            }
        }
    }

    public void ApplyFfbSens(IEnumerable<(int CarId, PersonalCarFfbSens FfbSens)> carIdToFfbSens)
    {
        foreach ((int carId, PersonalCarFfbSens carFfbSens) in carIdToFfbSens)
        {
            KeyDataCollection? section = Sections[$"car{carId}"];

            if (section == null)
            {
                continue;
            }

            void SetFfbSens(string surface, int? ffbSens)
                => section[$"forcefeedbacksensitivity{surface}"] = $"{ffbSens:d}";
            
            SetFfbSens("gravel", carFfbSens.Gravel);
            SetFfbSens("tarmac", carFfbSens.Tarmac);
            SetFfbSens("snow", carFfbSens.Snow);
        }
    }

    public async Task ExportCarsAsync(FileInfo file)
    {
        Dictionary<int, PersonalCarFfbSens> carIdToFfbSens = GetCars()
            .ToDictionary(car => car.CarId, car => car.FfbSens);

        try
        {
            await using Stream stream = file.OpenWrite();
            await JsonSerializer.SerializeAsync(stream, carIdToFfbSens);
        }
        catch (Exception e)
        {
            throw new PersonalDataException(
                string.Format(
                    ModelTexts.PersonalDataExportCarsErrorFormat,
                    e.Message),
                e);
        }
    }

    public async Task ImportCarsAsync(FileInfo file)
    {
        if (!file.Exists)
        {
            throw new PersonalDataException(
                string.Format(
                    ModelTexts.PersonalDataImportCarsNotFoundFormat,
                    file.FullName));
        }

        Dictionary<int, PersonalCarFfbSens>? carIdToFfbSens;

        try
        {
            await using Stream stream = file.OpenRead();
            carIdToFfbSens = await JsonSerializer.DeserializeAsync<Dictionary<int, PersonalCarFfbSens>>(stream);
        }
        catch (Exception e)
        {
            throw new PersonalDataException(
                string.Format(
                    ModelTexts.PersonalDataImportCarsErrorFormat,
                    file.FullName,
                    e.Message),
                e);
        }

        if (carIdToFfbSens == null)
        {
            throw new PersonalDataException(
                string.Format(
                    ModelTexts.PersonalDataImportCarsNullFormat,
                    file.FullName));
        }

        ApplyFfbSens(carIdToFfbSens.Select(kvp => (kvp.Key, kvp.Value)));
    }
}

