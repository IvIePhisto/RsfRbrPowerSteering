using System.Text.Json;

namespace RsfRbrPowerSteering.Settings;

public class RootSettings
{
    public static readonly FileInfo SettingsFile = new FileInfo($"{nameof(RsfRbrPowerSteering)}.json");

    public static async Task<RootSettings?> LoadAsync()
    {
        try
        {
            if (SettingsFile.Exists)
            {
                await using Stream stream = SettingsFile.OpenRead();

                return await JsonSerializer.DeserializeAsync<RootSettings>(stream);
            }

            return null;
        }
        catch (Exception e)
        {
            throw new SettingsException(
                string.Format(
                    SettingsTexts.RootSettingsLoadErrorFormat,
                    e.Message),
                e);
        }
    }

    public bool IsDescriptionVisible { get; set; }
    public int? TargetCarId { get; set; }
    public CarSettings PrimaryCar { get; set; } = null!;
    public CarSettings SecondaryCar { get; set;} = null!;
    public string PrimarySurface { get; set; } = string.Empty;
    public int AdjustmentWeightRatio { get; set; }
    public int AdjustmentFwd { get; set; }
    public int AdjustmentRwd { get; set; }
    public int AdjustmentAwd { get; set; }
    public int AdjustmentGravel { get; set; }
    public int AdjustmentTarmac { get; set; }
    public int AdjustmentSnow { get; set; }

    public async Task SaveAsync()
    {
        try
        {
            await using Stream stream = SettingsFile.Create();
            await JsonSerializer.SerializeAsync(
                stream,
                this,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }
        catch (Exception e)
        {
            throw new SettingsException(
                string.Format(
                    SettingsTexts.RootSettingsSaveErrorFormat,
                    e.Message),
                e);
        }
    }
}
