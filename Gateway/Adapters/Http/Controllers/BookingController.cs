using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;

namespace Gateway.Adapters.Http.Controllers;


public class BookingController : BookingApiController
{
    public override Task<IActionResult> BookVehicle(BookVehicleRequest bookVehicleRequest)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> CancelBookingVehicle(Guid bookingId)
    {
        throw new NotImplementedException();
    }
}