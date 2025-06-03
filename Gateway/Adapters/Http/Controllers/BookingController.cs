using Api;
using Clients.Clients.Booking;
using Gateway.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using Proto.IdentityV1;
using BookVehicleRequest = OpenApiContractV1.Models.BookVehicleRequest;
using BookVehicleResponse = OpenApiContractV1.Models.BookVehicleResponse;

namespace Gateway.Adapters.Http.Controllers;

[Authorization(Role.CustomerUnspecified)]
public class BookingController(BookingClientWrapper clientWrapper) : BookingApiController
{
    public override async Task<IActionResult> BookVehicle(BookVehicleRequest request)
    {
        var serviceResponse = await clientWrapper.BookVehicle(CreateGrpcRequest(request));

        return Ok(MapToResponse(serviceResponse));

        Api.BookVehicleRequest CreateGrpcRequest(BookVehicleRequest r)
        {
            return new Api.BookVehicleRequest
            {
                CustomerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "customer_id")?.Value, 
                VehicleId = r.VehicleId.ToString()
            };
        }
        
        BookVehicleResponse MapToResponse(Api.BookVehicleResponse grpcResponse)
        {
            return new BookVehicleResponse { BookingId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.BookingId) };
        }
    }

    public override async Task<IActionResult> CancelBookingVehicle(Guid bookingId)
    {
        await clientWrapper.CancelBooking(new CancelBookingRequest { BookingId = bookingId.ToString() });

        return Ok();
    }
}