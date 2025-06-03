using Api;
using Clients.Clients.Check;
using Gateway.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using AddDamageFixationsRequest = OpenApiContractV1.Models.AddDamageFixationsRequest;
using ByteString = Google.Protobuf.ByteString;
using FixationCategory = OpenApiContractV1.Models.FixationCategory;
using GetCheckByIdResponse = Api.GetCheckByIdResponse;
using Role = Proto.IdentityV1.Role;
using StartCheckRequest = OpenApiContractV1.Models.StartCheckRequest;
using StartCheckResponse = Api.StartCheckResponse;

namespace Gateway.Adapters.Http.Controllers;

[Authorization(Role.CustomerUnspecified)]
public class CheckController(CheckClientWrapper clientWrapper) : VehicleCheckApiController
{
    public override async Task<IActionResult> AddDamageFixationsToCheck(Guid checkId, AddDamageFixationsRequest request)
    {
        await clientWrapper.AddDamageFixations(CreateGrpcRequest(checkId, request));

        return Ok();

        Api.AddDamageFixationsRequest CreateGrpcRequest(Guid id, AddDamageFixationsRequest r)
        {
            var fixationCategoryMapper = new FixationCategoryEnumMapper();

            var grpcRequest = new Api.AddDamageFixationsRequest { CheckId = id.ToString() };

            grpcRequest.Fixations.AddRange(r.Fixations.Select(x => new Api.AddDamageFixationsRequest.Types.Fixation
            {
                Category = fixationCategoryMapper.ToProto(x.Category),
                Description = x.Description,
                PhotoBytes = ByteString.CopyFrom(x.PhotoBytes.AsSpan())
            }));

            return grpcRequest;
        }
    }

    public override async Task<IActionResult> CompleteCheck(Guid checkId)
    {
        await clientWrapper.CompleteCheck(new CompleteCheckRequest { CheckId = checkId.ToString() });

        return Ok();
    }

    public override async Task<IActionResult> GetCheckById(Guid checkId)
    {
        var serviceResponse =
            await clientWrapper.GetCheckById(new GetCheckByIdRequest { CheckId = checkId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        OpenApiContractV1.Models.GetCheckByIdResponse MapToResponse(GetCheckByIdResponse grpcResponse)
        {
            var fixationCategoryMapper = new FixationCategoryEnumMapper();

            return new OpenApiContractV1.Models.GetCheckByIdResponse
            {
                CheckId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.CheckId),
                BookingId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.BookingId),
                VehicleId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleId),
                Start = grpcResponse.Start.ToDateTime(),
                End = grpcResponse.End == null ? null : grpcResponse.End.ToDateTime(),
                Fixations = grpcResponse.Fixations.Select(x => new Fixation
                    {
                        Category = fixationCategoryMapper.ToContract(x.Category),
                        Description = x.Description,
                        PhotoS3Url = x.S3Url
                    })
                    .ToList()
            };
        }
    }

    public override async Task<IActionResult> StartCheck(StartCheckRequest request)
    {
        var serviceResponse = await clientWrapper.StartCheck(new Api.StartCheckRequest
            { BookingId = request.BookingId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        OpenApiContractV1.Models.StartCheckResponse MapToResponse(StartCheckResponse grpcResponse)
        {
            return new OpenApiContractV1.Models.StartCheckResponse
                { CheckId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.CheckId) };
        }
    }
}

public class FixationCategoryEnumMapper
{
    public FixationCategory ToContract(Api.FixationCategory c)
    {
        return c switch
        {
            Api.FixationCategory.MinorDamageUnspecified => FixationCategory.MinorDamageEnum,
            Api.FixationCategory.SignificantDamage => FixationCategory.SignificantDamageEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }

    public Api.FixationCategory ToProto(FixationCategory c)
    {
        return c switch
        {
            FixationCategory.MinorDamageEnum => Api.FixationCategory.MinorDamageUnspecified,
            FixationCategory.SignificantDamageEnum => Api.FixationCategory.SignificantDamage,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
}