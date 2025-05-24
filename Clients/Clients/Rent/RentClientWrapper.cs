using Api;
using Grpc.Core;

namespace Clients.Clients.Rent;

public class RentClientWrapper(Api.Rent.RentClient client)
{
    public async Task<StartRentResponse> StartRent(StartRentRequest request, CancellationToken ct = default)
    {
        return await client.StartRentAsync(request, new CallOptions(cancellationToken: ct));
    }

    public async Task<CompleteRentResponse> CompleteRent(CompleteRentRequest request, CancellationToken ct = default)
    {
        return await client.CompleteRentAsync(request, new CallOptions(cancellationToken: ct));
    }

    public async Task<GetCurrentAmountRentResponse> GetCurrentAmountRent(GetCurrentAmountRentRequest request, CancellationToken ct = default)
    {
        return await client.GetCurrentAmountRentAsync(request, new CallOptions(cancellationToken: ct));
    }
}