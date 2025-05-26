using Clients.Clients.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;

namespace Gateway.Adapters.Http.Controllers;

public class IdentityController(IdentityClientWrapper clientWrapper) : IdentityApiController
{
    public override async Task<IActionResult> Authenticate(AuthenticateRequest request)
    {
        var response = await clientWrapper.Authenticate(new Proto.IdentityV1.AuthenticateRequest
            { Email = request.Email, Pass = request.Password });

        return Ok(new AuthenticateResponse { AccessToken = response.Tkn, RefreshToken = response.RefreshTkn });
    }

    public override async Task<IActionResult> ConfirmAccountEmail(Guid accountId, string confirmationToken)
    {
        await clientWrapper.ConfirmEmail(new Proto.IdentityV1.ConfirmEmailRequest
            { AccId = accountId.ToString(), ConfirmationTkn = confirmationToken });

        return Ok();
    }

    public override async Task<IActionResult> CreateAccount(CreateRequest request)
    {
        var roleMapper = new RoleEnumMapper();

        var response = await clientWrapper.CreateAccount(new Proto.IdentityV1.CreateRequest
        {
            Email = request.Email,
            Pass = request.Password,
            Phone = request.Phone,
            Role = roleMapper.ToProto(request.Role)
        });
        
        return Ok(new CreateResponse { AccountId = Guid.Parse(response.AccId) });
    }

    public override async Task<IActionResult> RecoverPassword(string accountId, RecoverPasswordRequest request)
    {
        await clientWrapper.RecoverPassword(new Proto.IdentityV1.RecoverPasswordRequest
            { Email = request.Email });

        return Ok();
    }

    public override async Task<IActionResult> RefreshAccessToken(RefreshAccessTokenRequest request)
    {
        var response = await clientWrapper.RefreshAccessToken(new Proto.IdentityV1.RefreshAccessTokenRequest
            { RefreshTkn = request.RefreshToken });

        return Ok(new RefreshAccessTokenResponse { AccessToken = response.Tkn, RefreshToken = response.RefreshTkn });
    }

    public override async Task<IActionResult> UpdatePassword(string accountId, UpdatePasswordRequest request)
    {
        await clientWrapper.UpdatePassword(new Proto.IdentityV1.UpdatePasswordRequest
            { Email = request.Email, ResetTkn = request.ResetToken, NewPass = request.NewPassword });

        return Ok();
    }
}

class RoleEnumMapper
{
    public Role ToContract(Proto.IdentityV1.Role r)
    {
        return r switch
        {
            Proto.IdentityV1.Role.CustomerUnspecified => Role.CustomerEnum,
            Proto.IdentityV1.Role.Admin => Role.AdminEnum,
            Proto.IdentityV1.Role.Support => Role.SupportEnum,
            Proto.IdentityV1.Role.Maintenance => Role.MaintenanceEnum,
            Proto.IdentityV1.Role.Hr => Role.HrEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
        };
    }

    public Proto.IdentityV1.Role ToProto(Role r)
    {
        return r switch
        {
            Role.CustomerEnum => Proto.IdentityV1.Role.CustomerUnspecified,
            Role.AdminEnum => Proto.IdentityV1.Role.Admin,
            Role.SupportEnum => Proto.IdentityV1.Role.Support,
            Role.MaintenanceEnum => Proto.IdentityV1.Role.Maintenance,
            Role.HrEnum => Proto.IdentityV1.Role.Hr,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
        };
    }
}