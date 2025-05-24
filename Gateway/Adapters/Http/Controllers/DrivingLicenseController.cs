using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;

namespace Gateway.Adapters.Http.Controllers;

public class DrivingLicenseController : DrivingLicenseApiController
{
    public override Task<IActionResult> GetDrivingLicenseById(Guid licenseId)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> UploadDrivingLicense(UploadLicenseRequest uploadLicenseRequest)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> UploadDrivingLicensePhotos(Guid licenseId, UploadPhotosRequest uploadPhotosRequest)
    {
        throw new NotImplementedException();
    }
}