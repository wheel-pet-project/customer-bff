using Grpc.Core;
using Proto.DrivingLicenseV1;

namespace Clients.Clients.DrivingLicense;

public class DrivingLicenseClientWrapper(Proto.DrivingLicenseV1.DrivingLicense.DrivingLicenseClient client)
{
    public async Task<UploadLicenseResponse> UploadLicense(UploadLicenseRequest request, CancellationToken ct = default)
    {
        return await client.UploadLicenseAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<UploadPhotosResponse> UploadPhotos(UploadPhotosRequest request, CancellationToken ct = default)
    {
        return await client.UploadPhotosAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetLicenseByIdResponse> GetLicenseById(
        GetLicenseByIdRequest request,
        CancellationToken ct = default)
    {
        return await client.GetLicenseByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }
}