using System.Linq.Expressions;
using Blogger.Contracts.Data.Entities;
using Blogger.Contracts.Models;

namespace Blogger.Contracts.Services;

public interface IBlogsService
{
    public Task<IEnumerable<BlogDTO>> GetBlogsAsync(Expression<Func<Blog, bool>> predicate, int skipCount,
        int takeCount);

    public Task<Blog> GetBlogAsync(int id);
    public Task<Blog> CreateBlogAsync(BlogDTO blog);
    public Task<Blog> UpdateBlogAsync(int id, BlogDTO blog);
    public Task DeleteBlogAsync(int id);
    public Task SoftDeleteBlogAsync(int id);
    public Task<int> CountAsync(Expression<Func<Blog, bool>> predicate);
    public Task<int> CountAsync();
}