using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/check")]
public class CheckController : ControllerBase
{
    
}