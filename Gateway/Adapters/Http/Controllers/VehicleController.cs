using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;

namespace Gateway.Adapters.Http.Controllers;

public class VehicleController : VehicleApiController
{
    public override Task<IActionResult> GetVehicleById(string vehicleId)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> GetVehiclesInSquare(Status status, Location upperLeftLocation, Location lowerRightLocation)
    {
        throw new NotImplementedException();
    }
}