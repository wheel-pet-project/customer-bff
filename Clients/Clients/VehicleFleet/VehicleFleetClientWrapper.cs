using Grpc.Core;
using Proto.VehicleFleetV1;

namespace Clients.Clients.VehicleFleet;

public class VehicleFleetClientWrapper(Proto.VehicleFleetV1.VehicleFleet.VehicleFleetClient client)
{
    public async Task<GetVehiclesInSquareRes> GetVehiclesInSquare(
        GetVehiclesInSquareReq request,
        CancellationToken ct = default)
    {
        return await client.GetVehiclesInSquareAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetVehicleByIdRes> GetVehicleById(GetVehicleByIdReq request, CancellationToken ct = default)
    {
        return await client.GetVehicleByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetAllModelsRes> GetAllModels(GetAllModelsReq request, CancellationToken ct = default)
    {
        return await client.GetAllModelsAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetModelByIdRes> GetModelById(GetModelByIdReq request, CancellationToken ct = default)
    {
        return await client.GetModelByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }
}