namespace RsfRbrPowerSteering.Model.Rsf;

public static class CarInfoExtensions
{
    public static PersonalCarFfbSens ToPersonal(this CarFfbSens ffbSens)
        => new()
            {
                Gravel = ffbSens.Gravel,
                Tarmac = ffbSens.Tarmac,
                Snow = ffbSens.Snow
            };
}
