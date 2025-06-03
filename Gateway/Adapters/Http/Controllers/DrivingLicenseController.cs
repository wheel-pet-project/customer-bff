using Clients.Clients.DrivingLicense;
using Gateway.Authorization;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using Proto.DrivingLicenseV1;
using Proto.IdentityV1;
using UploadLicenseRequest = OpenApiContractV1.Models.UploadLicenseRequest;
using UploadPhotosRequest = OpenApiContractV1.Models.UploadPhotosRequest;

namespace Gateway.Adapters.Http.Controllers;

[Authorization(Role.CustomerUnspecified)]
public class DrivingLicenseController(DrivingLicenseClientWrapper clientWrapper) : DrivingLicenseApiController
{
    public override async Task<IActionResult> GetDrivingLicenseById(Guid licenseId)
    {
        var serviceResponse =
            await clientWrapper.GetLicenseById(new GetLicenseByIdRequest { Id = licenseId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        OpenApiContractV1.Models.GetLicenseByIdResponse MapToResponse(GetLicenseByIdResponse grpcResponse)
        {
            return new OpenApiContractV1.Models.GetLicenseByIdResponse
            {
                Id = Guid.Parse((ReadOnlySpan<char>)grpcResponse.Id),
                AccountId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.AccId),
                Categories = [..grpcResponse.Categories],
                CodeOfIssue = grpcResponse.CodeOfIssue,
                CityOfBirth = grpcResponse.CityOfBirth,
                DateOfBirth = DateOnly.FromDateTime(grpcResponse.DateOfBirth.ToDateTime()),
                DateOfIssue = DateOnly.FromDateTime(grpcResponse.DateOfIssue.ToDateTime()),
                DateOfExpiry = DateOnly.FromDateTime(grpcResponse.DateOfExpiry.ToDateTime()),
                FrontPhotoS3Url = string.IsNullOrWhiteSpace(serviceResponse.FrontPhotoS3Url) ? null! : serviceResponse.FrontPhotoS3Url,
                BackPhotoS3Url = string.IsNullOrWhiteSpace(serviceResponse.BackPhotoS3Url) ? null! : serviceResponse.BackPhotoS3Url,
            };
        }
    }

    public override async Task<IActionResult> UploadDrivingLicense(UploadLicenseRequest request)
    {
        var serviceResponse = await clientWrapper.UploadLicense(CreateGrpcRequest(request));

        return Ok(MapToResponse(serviceResponse));
        
        Proto.DrivingLicenseV1.UploadLicenseRequest CreateGrpcRequest(UploadLicenseRequest r)
        {
            var grpcRequest = new Proto.DrivingLicenseV1.UploadLicenseRequest
            {
                AccId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "customer_id")?.Value,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Patronymic = r.Patronymic,
                Number = r.Number,
                CityOfBirth = r.CityOfBirth,
                DateOfBirth = Timestamp.FromDateTime(r.DateOfBirth.ToDateTime(new TimeOnly(), DateTimeKind.Utc)),
                CodeOfIssue = r.CodeOfIssue,
                DateOfIssue = Timestamp.FromDateTime(r.DateOfIssue.ToDateTime(new TimeOnly(), DateTimeKind.Utc)),
                DateOfExpiry = Timestamp.FromDateTime(r.DateOfExpiry.ToDateTime(new TimeOnly(), DateTimeKind.Utc)),
            };
            
            grpcRequest.Categories.AddRange(r.Categories);

            return grpcRequest;
        }
        
        
        OpenApiContractV1.Models.UploadLicenseResponse MapToResponse(UploadLicenseResponse grpcResponse)
        {
            return new OpenApiContractV1.Models.UploadLicenseResponse { Id = Guid.Parse((ReadOnlySpan<char>)grpcResponse.Id), };
        }
    }

    public override async Task<IActionResult> UploadDrivingLicensePhotos(Guid licenseId, UploadPhotosRequest request)
    {
        await clientWrapper.UploadPhotos(CreateGrpcRequest(licenseId, request));

        return Ok();

        Proto.DrivingLicenseV1.UploadPhotosRequest CreateGrpcRequest(Guid id, UploadPhotosRequest r)
        {
            return new Proto.DrivingLicenseV1.UploadPhotosRequest
            {
                LicenseId = id.ToString(),
                FrontPhoto = ByteString.CopyFrom(r.FrontPhotoBytes.AsSpan()),
                BackPhoto = ByteString.CopyFrom(r.BackPhotoBytes.AsSpan())
            };
        }
    }
}