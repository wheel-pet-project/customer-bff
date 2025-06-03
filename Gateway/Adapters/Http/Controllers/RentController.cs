
using Api;
using Clients.Clients.Rent;
using Gateway.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using Proto.IdentityV1;
using GetCurrentAmountRentResponse = OpenApiContractV1.Models.GetCurrentAmountRentResponse;
using StartRentRequest = OpenApiContractV1.Models.StartRentRequest;

namespace Gateway.Adapters.Http.Controllers;

[Authorization(Role.CustomerUnspecified)]
public class RentController(RentClientWrapper clientWrapper) : RentApiController
{
    public override async Task<IActionResult> CompleteRent(Guid rentId)
    {
        await clientWrapper.CompleteRent(new CompleteRentRequest { RentId = rentId.ToString() });

        return Ok();
    }

    public override async Task<IActionResult> GetCurrentAmountForRent(Guid rentId)
    {
        var serviceResponse = await clientWrapper.GetCurrentAmountRent(new GetCurrentAmountRentRequest { RentId = rentId.ToString() });

        return Ok(MapToResponse(serviceResponse));
        
        GetCurrentAmountRentResponse MapToResponse(Api.GetCurrentAmountRentResponse grpcResponse)
        {
            return new GetCurrentAmountRentResponse
            {
                RentId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.RentId),
                CurrentAmount = grpcResponse.CurrentAmount
            };
        }
    }

    public override async Task<IActionResult> StartRent(StartRentRequest request)
    {
        var serviceResponse = await clientWrapper.StartRent(new Api.StartRentRequest
        {
            BookingId = request.BookingId.ToString(), 
            CustomerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "customer_id")?.Value,
            VehicleId = request.VehicleId.ToString()
        });

        return Ok(MapToResponse(serviceResponse));

        OpenApiContractV1.Models.StartRentResponse MapToResponse(StartRentResponse grpcResponse)
        {
            return new OpenApiContractV1.Models.StartRentResponse { RentId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.RentId) };
        }
    }
}