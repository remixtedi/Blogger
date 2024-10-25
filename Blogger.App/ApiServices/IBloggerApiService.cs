using Blogger.Contracts.Models;
using Blogger.Contracts.Models.Requests;
using Blogger.Contracts.Models.Responses;

namespace Blogger.App.ApiServices;

public interface IBloggerApiService
{
    Task<FilterBlogsResult> GetBlogs(FilterBlogsRequest filterBlogsRequest);
    Task<BlogDTO> GetBlogById(int id);
    Task<BlogDTO> CreateBlog(BlogDTO blog);
    Task<BlogDTO> UpdateBlog(int id, BlogDTO blog);
    Task DeleteBlog(int id);
}