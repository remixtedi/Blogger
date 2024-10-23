using Blogger.Contracts.Data.Repositories;

namespace Blogger.Contracts.Data;

public interface IUnitOfWork
{
    public IBlogRepository Blogs { get; }
    public Task SaveChangesAsync();
}