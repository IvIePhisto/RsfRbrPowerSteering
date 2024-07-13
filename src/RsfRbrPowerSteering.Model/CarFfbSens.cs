namespace RsfRbrPowerSteering.Model;

public struct CarFfbSens
{
    public int? Gravel { get; set; }
    public int? Tarmac { get; set; }
    public int? Snow { get; set; }

    private static int? ScaleAndRoundFfbSens(int? sensitivity, decimal factor)
    {
        if (sensitivity.HasValue)
        {
            int result = Convert.ToInt32(Math.Round(sensitivity.Value * factor));

            return result <= 0
                ? 1
                : result;
        }
        else
        {
            return null;
        }
    }

    public CarFfbSens Normalize(decimal factor)
        => new CarFfbSens
        {
            Gravel = ScaleAndRoundFfbSens(Gravel, factor),
            Tarmac = ScaleAndRoundFfbSens(Tarmac, factor),
            Snow = ScaleAndRoundFfbSens(Snow, factor)
        };
}
