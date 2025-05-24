using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;

namespace Gateway.Adapters.Http.Controllers;

public class CheckController : VehicleCheckApiController
{
    public override Task<IActionResult> AddDamageFixationsToCheck(Guid checkId, AddDamageFixationsRequest addDamageFixationsRequest)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> CompleteCheck(Guid checkId)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> GetCheckById(Guid checkId)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> StartCheck(StartCheckRequest startCheckRequest)
    {
        throw new NotImplementedException();
    }
}