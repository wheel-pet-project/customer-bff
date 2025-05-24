using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;

namespace Gateway.Adapters.Http.Controllers;

public class IdentityController : IdentityApiController
{
    public override Task<IActionResult> Authenticate(AuthenticateRequest authenticateRequest)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> ConfirmAccountEmail(Guid accountId, string confirmationToken, ConfirmEmailRequest confirmEmailRequest)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> CreateAccount(CreateRequest createRequest)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> RecoverPassword(string accountId, RecoverPasswordRequest recoverPasswordRequest)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> RefreshAccessToken(RefreshAccessTokenRequest refreshAccessTokenRequest)
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> UpdatePassword(string accountId, UpdatePasswordRequest updatePasswordRequest)
    {
        throw new NotImplementedException();
    }
}