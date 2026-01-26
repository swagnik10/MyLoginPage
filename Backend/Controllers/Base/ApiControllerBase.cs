using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyApp.Controllers.Base;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected int GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("UserId claim missing");

        return int.Parse(userId);
    }
}
