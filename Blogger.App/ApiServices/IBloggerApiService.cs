using Blogger.Contracts.Models;

namespace Blogger.App.ApiServices;

public interface IBloggerApiService
{
    Task<IEnumerable<BlogDTO>> GetBlogs();
    Task<BlogDTO> GetBlogById(int id);
    Task<BlogDTO> CreateBlog(BlogDTO blog);
    Task<BlogDTO> UpdateBlog(int id, BlogDTO blog);
    Task DeleteBlog(int id);
}