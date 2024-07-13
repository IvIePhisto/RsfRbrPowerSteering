using RsfRbrPowerSteering.Model.Rsf;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace RsfRbrPowerSteering.Model;

public struct CarInfo
{
    private static async Task<JsonDocument> ReadJsonFileAsync(FileInfo file, CancellationToken cancellationToken)
    {
        using FileStream fileStream = file.OpenRead();

        return await JsonDocument.ParseAsync(fileStream, cancellationToken: cancellationToken);
    }

    private static async IAsyncEnumerable<CarInfo> EnumerateCarsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var carsById = new Dictionary<int, JsonElement>();
        using JsonDocument cars = await ReadJsonFileAsync(RsfFiles.CarsFile, cancellationToken);

        foreach (JsonElement car in cars.RootElement.EnumerateArray())
        {
            string idRaw = car.GetProperty("id").GetString()!;
            var id = int.Parse(idRaw);
            carsById[id] = car;
        }

        using JsonDocument carsData = await ReadJsonFileAsync(RsfFiles.CarsDataFile, cancellationToken);

        foreach (JsonElement carData in carsData.RootElement.EnumerateArray())
        {
            string idRaw = carData.GetProperty("car_id").GetString()!;
            var id = int.Parse(idRaw);

            if (id == 0)
            {
                continue;
            }

            JsonElement car = carsById[id];

            string name = car.GetProperty("name").GetString()!;

            string lockToLockRotationRaw = carData.GetProperty("steering_wheel").GetString()!;
            var lockToLockRotation = int.Parse(lockToLockRotationRaw);

            string weightRaw = carData.GetProperty("weight").GetString()!;
            var weightKg = int.Parse(weightRaw[..(weightRaw.Length - 2)]); // strip "kg" at end.

            string drivetrainRaw = carData.GetProperty("drive_train").GetString()![..3];
            var drivetrain = Enum.Parse<Drivetrain>(drivetrainRaw, true);

            yield return new CarInfo(
                id,
                name,
                lockToLockRotation,
                weightKg,
                drivetrain);
        }
    }

    public static async Task<IReadOnlyDictionary<int, CarInfo>> ReadCarsAsync(CancellationToken cancellationToken = default)
    {
        var carInfos = new Dictionary<int, CarInfo>();

        await foreach (CarInfo carInfo in EnumerateCarsAsync(cancellationToken))
        {
            carInfos[carInfo.Id] = carInfo;
        }

        return carInfos;
    }

    private CarInfo(
        int id,
        string name,
        int lockToLockRotation,
        int weightKg,
        Drivetrain drivetrain)
    {
        Id = id;
        Name = name;
        LockToLockRotation = lockToLockRotation;
        WeightKg = weightKg;
        Drivetrain = drivetrain;
    }

    public int Id { get; }
    public string Name { get; }
    public int LockToLockRotation { get; }
    public int WeightKg { get; }
    public Drivetrain Drivetrain { get; }
}
