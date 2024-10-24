using System.Net.Http.Headers;

namespace Blogger.App.Auth;

public class AuthenticatedHttpClientHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor?.HttpContext?.Request.Cookies["BloggerAuthToken"];
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}