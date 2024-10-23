using Blogger.Contracts.Data.Entities;
using Blogger.Contracts.Data.Repositories;
using Blogger.Migrations;

namespace Blogger.Infrastructure.Data.Repositories;

public class BlogRepository(ApplicationDbContext context) : Repository<Blog>(context), IBlogRepository;