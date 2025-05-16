using Grpc.Net.Client;
using Proto.IdentityV1;

namespace Clients.Identity;

public class IdentityClient
{
    public async Task CreateAccount()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7042");
        var client = new Proto.IdentityV1.Identity.IdentityClient(channel);
        var response = await client.CreateAccountAsync(new CreateRequest
        {
            Role = Role.CustomerUnspecified,
            Email = null,
            Phone = null,
            Pass = null
        });
    }
}