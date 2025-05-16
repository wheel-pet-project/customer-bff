using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;


[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/booking")]
public class BookingController : ControllerBase
{
    
}