using System.Security.Claims;
using Blogger.Contracts.Services;
using Microsoft.AspNetCore.Http;

namespace Blogger.Infrastructure.Services;

public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
    public bool IsAuthorized
    {
        get
        {
            var context = httpContextAccessor.HttpContext;
            return context?.User is { Identity.IsAuthenticated: true };
        }
    }

    public string UserId
    {
        get
        {
            var context = httpContextAccessor.HttpContext;
            if (context?.User is not { Identity.IsAuthenticated: true }) return string.Empty;
            var identifier = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return identifier != null ? identifier.Value : string.Empty;
        }
    }
}