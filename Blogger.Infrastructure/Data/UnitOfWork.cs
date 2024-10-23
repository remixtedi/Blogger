using Blogger.Contracts.Data;
using Blogger.Contracts.Data.Repositories;
using Blogger.Infrastructure.Data.Repositories;
using Blogger.Migrations;

namespace Blogger.Infrastructure.Data;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public IBlogRepository Blogs  => new BlogRepository(context);

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}