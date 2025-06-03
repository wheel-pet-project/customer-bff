using Api;
using Grpc.Core;

namespace Clients.Clients.Check;

public class CheckClientWrapper(VehicleCheck.VehicleCheckClient client)
{
    public async Task<StartCheckResponse> StartCheck(StartCheckRequest request, CancellationToken ct = default)
    {
        return await client.StartCheckAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<CompleteCheckResponse> CompleteCheck(CompleteCheckRequest request, CancellationToken ct = default)
    {
        return await client.CompleteCheckAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<AddDamageFixationsResponse> AddDamageFixations(
        AddDamageFixationsRequest request,
        CancellationToken ct = default)
    {
        return await client.AddDamageFixationsAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetCheckByIdResponse> GetCheckById(GetCheckByIdRequest request, CancellationToken ct = default)
    {
        return await client.GetCheckByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }
}