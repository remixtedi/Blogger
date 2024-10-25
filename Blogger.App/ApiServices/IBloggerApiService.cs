using Blogger.Contracts.Models;

namespace Blogger.App.ApiServices;

public interface IBloggerApiService
{
    Task<IEnumerable<BlogDTO>> GetBlogs();
    Task<BlogDTO> GetBlogById(int id);
}