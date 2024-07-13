namespace RsfRbrPowerSteering.Model.Calculation;

public static class CalculationUtility
{
    private struct FfbSensCalculationParameters
    {
        private readonly struct Parameters
        {
            public Parameters(
                decimal? ffbSensA,
                decimal? ffbSensB,
                decimal valueA,
                decimal valueB,
                decimal drivetrainFactorA,
                decimal drivetrainFactorB)
            {
                ffbSensA /= drivetrainFactorA;
                ffbSensB /= drivetrainFactorB;

                Factor = valueA == valueB
                    ? 1M
                    : (ffbSensA - ffbSensB) / (valueA - valueB);
                Offset = ffbSensA - valueA * Factor;
            }

            public decimal? Factor { get; }
            public decimal? Offset { get; }


            public int? Calculate(decimal value)
                => Factor.HasValue && Offset.HasValue
                    ? Convert.ToInt32(value * Factor + Offset)
                    : null;
        }

        public FfbSensCalculationParameters(
            CalculationCar carA,
            CalculationCar carB,
            DrivetrainFactors drivetrainFactors)
        {
            decimal drivetrainA = drivetrainFactors[carA.Drivetrain];
            decimal drivetrainB = drivetrainFactors[carB.Drivetrain];
            const bool makeFirstGreaterLockToLockRotation = false;
            const bool makeFirstGreatWeight = !makeFirstGreaterLockToLockRotation;
            ParametersLockToLockRotationGravel = new Parameters(
                carA.FfbSens.Gravel,
                carB.FfbSens.Gravel,
                carA.MaxSteeringLock,
                carB.MaxSteeringLock,
                drivetrainA,
                drivetrainB);
            ParametersLockToLockRotationTarmac = new Parameters(
                carA.FfbSens.Tarmac,
                carB.FfbSens.Tarmac,
                carA.MaxSteeringLock,
                carB.MaxSteeringLock,
                drivetrainA,
                drivetrainB);
            ParametersLockToLockRotationSnow = new Parameters(
                carA.FfbSens.Snow,
                carB.FfbSens.Snow,
                carA.MaxSteeringLock,
                carB.MaxSteeringLock,
                drivetrainA,
                drivetrainB);
            ParametersWeightKgGravel = new Parameters(
                carA.FfbSens.Gravel,
                carB.FfbSens.Gravel,
                carA.WeightKg,
                carB.WeightKg,
                drivetrainA,
                drivetrainB);
            ParametersWeightKgTarmac = new Parameters(
                carA.FfbSens.Tarmac,
                carB.FfbSens.Tarmac,
                carA.WeightKg,
                carB.WeightKg,
                drivetrainA,
                drivetrainB);
            ParametersWeightKgSnow = new Parameters(
                carA.FfbSens.Snow,
                carB.FfbSens.Snow,
                carA.WeightKg,
                carB.WeightKg,
                drivetrainA,
                drivetrainB);
        }

        private Parameters ParametersLockToLockRotationGravel { get; }
        private Parameters ParametersLockToLockRotationTarmac { get; }
        private Parameters ParametersLockToLockRotationSnow { get; }

        private Parameters ParametersWeightKgGravel { get; }
        private Parameters ParametersWeightKgTarmac { get; }
        private Parameters ParametersWeightKgSnow { get; }

        public CarFfbSens Calculate(int lockToLockRotation, int weightKg, decimal weightRatio)
        {
            int? calculateForSurface(Parameters parametersLockToLockRotation, Parameters parametersWeightKg)
            {
                if (weightRatio == 0)
                {
                    return parametersLockToLockRotation.Calculate(lockToLockRotation);
                }
                else if (weightRatio == 1)
                {
                    return parametersWeightKg.Calculate(weightKg);
                }
                else
                {
                    decimal? ffbSens = (1 - weightRatio) * parametersLockToLockRotation.Calculate(lockToLockRotation)
                        + weightRatio * parametersWeightKg.Calculate(weightKg);

                    return ffbSens.HasValue
                        ? Convert.ToInt32(Math.Round(ffbSens.Value))
                        : null;
                }
            }

            return new CarFfbSens
            {
                Gravel = calculateForSurface(ParametersLockToLockRotationGravel, ParametersWeightKgGravel),
                Tarmac = calculateForSurface(ParametersLockToLockRotationTarmac, ParametersWeightKgTarmac),
                Snow = calculateForSurface(ParametersLockToLockRotationSnow, ParametersWeightKgSnow)
            };
        }
    };

    public static IReadOnlyDictionary<int, CarFfbSens> CalculateFfbSenses(
        IEnumerable<CarInfo> cars,
        decimal weightRatio,
        DrivetrainFactors drivetrainFactors,
        CalculationCar carA,
        CalculationCar carB)
    {
        if (weightRatio < 0 || weightRatio > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(weightRatio), ModelTexts.WeightRatioRangeError);
        }

        var ffbSensCalcParams = new FfbSensCalculationParameters(
            carA,
            carB,
            drivetrainFactors);

        return cars
            .GroupBy(c => (c.Drivetrain, c.LockToLockRotation, c.WeightKg))
            .SelectMany(g =>
            {
                decimal drivetrainFactor = drivetrainFactors[g.Key.Drivetrain];
                CarFfbSens ffbSens = ffbSensCalcParams
                    .Calculate(g.Key.LockToLockRotation, g.Key.WeightKg, weightRatio)
                    .Normalize(drivetrainFactor);

                return g.Select(c => (c.Id, FfbSens: ffbSens));
            })
            .ToDictionary(i => i.Id, i => i.FfbSens);
    }
}
