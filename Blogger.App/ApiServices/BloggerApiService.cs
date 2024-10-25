using System.Net;
using Blogger.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Blogger.App.ApiServices;

public class BloggerApiService(
    ILogger<BloggerApiService> logger,
    NavigationManager navigationManager,
    HttpClient httpClient) : IBloggerApiService
{
    public async Task<IEnumerable<BlogDTO>> GetBlogs()
    {
        try
        {
            var response = await httpClient.GetAsync("blogs");

            logger.LogInformation($"Response status: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo(
                    $"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(navigationManager.Uri)}", true);
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

    public async Task<BlogDTO> GetBlogById(int id)
    {
        try
        {
            var response = await httpClient.GetAsync($"blogs/{id}");

            logger.LogInformation($"Response status: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo(
                    $"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(navigationManager.Uri)}", true);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogError($"API error: {response.StatusCode}, Content: {content}");
                throw new HttpRequestException($"API error: {response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<BlogDTO>()
                   ?? null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching blog");
            throw;
        }
    }

    public async Task<BlogDTO> CreateBlog(BlogDTO blog)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("blogs", blog);

            logger.LogInformation($"Response status: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo(
                    $"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(navigationManager.Uri)}", true);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogError($"API error: {response.StatusCode}, Content: {content}");
                throw new HttpRequestException($"API error: {response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<BlogDTO>()
                   ?? null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating blog");
            throw;
        }
    }

    public async Task<BlogDTO> UpdateBlog(int id, BlogDTO blog)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"blogs/{id}", blog);

            logger.LogInformation($"Response status: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo(
                    $"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(navigationManager.Uri)}", true);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogError($"API error: {response.StatusCode}, Content: {content}");
                throw new HttpRequestException($"API error: {response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<BlogDTO>()
                   ?? null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating blog");
            throw;
        }
    }

    public async Task DeleteBlog(int id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"blogs/{id}");

            logger.LogInformation($"Response status: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo(
                    $"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(navigationManager.Uri)}", true);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogError($"API error: {response.StatusCode}, Content: {content}");
                throw new HttpRequestException($"API error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting blog");
            throw;
        }
    }
}