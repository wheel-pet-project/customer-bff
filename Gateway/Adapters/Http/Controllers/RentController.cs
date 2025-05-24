
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;

namespace Gateway.Adapters.Http.Controllers;

public class RentController : RentApiController
{
    public override Task<IActionResult> CompleteRent(Guid rentId)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> GetCurrentAmountForRent(Guid rentId)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> StartRent(StartRentRequest startRentRequest)
    {
        throw new NotImplementedException();
    }
}