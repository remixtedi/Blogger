using System.Net;
using Blogger.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Blogger.App.ApiServices;

public class BloggerApiService(ILogger<BloggerApiService> logger, NavigationManager navigationManager, HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : IBloggerApiService
{
    public async Task<IEnumerable<BlogDTO>> GetBlogs()
    {
        try
        {
            var response = await httpClient.GetAsync("blogs");
            
            logger.LogInformation($"Response status: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo($"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(navigationManager.Uri)}", true);
                return Array.Empty<BlogDTO>();
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogError($"API error: {response.StatusCode}, Content: {content}");
                throw new HttpRequestException($"API error: {response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<BlogDTO>>() 
                   ?? Array.Empty<BlogDTO>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching blogs");
            throw;
        }
        
        
        return await httpClient.GetFromJsonAsync<IEnumerable<BlogDTO>>("blogs");
    }
}