using Clients.Clients.VehicleFleet;
using Gateway.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using Proto.VehicleFleetV1;
using Role = Proto.IdentityV1.Role;

namespace Gateway.Adapters.Http.Controllers;

[Authorization(Role.CustomerUnspecified)]
public class ModelController(VehicleFleetClientWrapper clientWrapper) : ModelApiController
{
    public override async Task<IActionResult> GetAllModels(int page, int pageSize)
    {
        var serviceResponse = await clientWrapper.GetAllModels(new GetAllModelsReq { Page = page, PageSize = pageSize });

        return Ok(MapToResponse(serviceResponse));

        GetAllModelsResponse MapToResponse(GetAllModelsRes grpcResponse)
        {
            return new GetAllModelsResponse
            {
                Models = grpcResponse.Models
                    .Select(x => new ModelShortView { ModelId = Guid.Parse((ReadOnlySpan<char>)x.ModelId), Brand = x.Brand, CarModel = x.CarModel })
                    .ToList()
            };
        }
    }

    public override async Task<IActionResult> GetModelById(Guid modelId)
    {
        var serviceResponse = await clientWrapper.GetModelById(new GetModelByIdReq { ModelId = modelId.ToString() });
        
        return Ok(MapToResponse(serviceResponse));

        GetModelByIdResponse MapToResponse(GetModelByIdRes grpcResponse)
        {
            return new GetModelByIdResponse
            {
                ModelId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.ModelId),
                Brand = grpcResponse.Brand,
                CarModel = grpcResponse.CarModel,
                Category = grpcResponse.Category,
                PricePerMinute = new decimal(grpcResponse.PricePerMin),
                PricePerHour = new decimal(grpcResponse.PricePerHour),
                PricePerDay = new decimal(grpcResponse.PricePerDay)
            };
        }
    }
}