using System.Linq.Expressions;
using AutoMapper;
using Blogger.Contracts.Data;
using Blogger.Contracts.Data.Entities;
using Blogger.Contracts.Enums;
using Blogger.Contracts.Models;
using Blogger.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Blogger.Infrastructure.Services;

public class BlogsService(ILogger<BlogsService> logger, IMapper mapper, IUnitOfWork unitOfWork) : IBlogsService
{
    public async Task<IEnumerable<Blog>> GetBlogsAsync(Expression<Func<Blog, bool>> predicate)
    {
        logger.LogInformation("Retrieving all blogs");
        try
        {
            var blogs = await unitOfWork.Blogs.GetAllAsync(predicate, e => e.Created, true);
            logger.LogInformation("Retrieved all blogs");
            return blogs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving blogs");
            throw;
        }
    }

    public async Task<Blog> GetBlogAsync(int id)
    {
        logger.LogInformation("Retrieving blog with ID: {BlogId}", id);
        try
        {
            var blog = await unitOfWork.Blogs.GetAsync(e => e.Id == id && e.EntityStatus == EntityStatus.Active);
            if (blog == null)
            {
                logger.LogWarning("Blog not found with ID: {BlogId}", id);
                throw new Exception("Blog not found");
            }

            logger.LogInformation("Retrieved blog: {BlogId} - {Title}", id, blog.Title);
            return blog;
        }
        catch (Exception ex) when (ex.Message != "Blog not found")
        {
            logger.LogError(ex, "Error retrieving blog with ID: {BlogId}", id);
            throw;
        }
    }

    public async Task<Blog> CreateBlogAsync(BlogDTO blog)
    {
        logger.LogInformation("Creating new blog with title: {Title}", blog.Title);
        try
        {
            var entity = mapper.Map<Blog>(blog);

            await unitOfWork.Blogs.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Created new blog: {Title}", blog.Title);
            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating blog with title: {Title}", blog.Title);
            throw;
        }
    }

    public async Task<Blog> UpdateBlogAsync(int id, BlogDTO blog)
    {
        logger.LogInformation("Updating blog with ID: {BlogId}", id);
        try
        {
            var existingBlog = await unitOfWork.Blogs.GetByIdAsync(id);
            if (existingBlog == null)
            {
                logger.LogWarning("Blog not found for update with ID: {BlogId}", id);
                throw new Exception("Blog not found");
            }

            existingBlog.Title = blog.Title;
            existingBlog.Content = blog.Content;

            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Updated blog: {BlogId} - {Title}", id, blog.Title);
            return existingBlog;
        }
        catch (Exception ex) when (ex.Message != "Blog not found")
        {
            logger.LogError(ex, "Error updating blog with ID: {BlogId}", id);
            throw;
        }
    }

    public async Task DeleteBlogAsync(int id)
    {
        logger.LogInformation("Deleting blog with ID: {BlogId}", id);
        try
        {
            var blog = await unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null)
            {
                logger.LogWarning("Blog not found for deletion with ID: {BlogId}", id);
                throw new Exception("Blog not found");
            }

            await unitOfWork.Blogs.DeleteAsync(blog.Id);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Deleted blog: {BlogId}", id);
        }
        catch (Exception ex) when (ex.Message != "Blog not found")
        {
            logger.LogError(ex, "Error deleting blog with ID: {BlogId}", id);
            throw;
        }
    }

    public async Task SoftDeleteBlogAsync(int id)
    {
        logger.LogInformation("Soft deleting blog with ID: {BlogId}", id);
        try
        {
            var blog = await unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null)
            {
                logger.LogWarning("Blog not found for soft deletion with ID: {BlogId}", id);
                throw new Exception("Blog not found");
            }

            blog.EntityStatus = EntityStatus.Deleted;
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Soft deleted blog: {BlogId}", id);
        }
        catch (Exception ex) when (ex.Message != "Blog not found")
        {
            logger.LogError(ex, "Error soft deleting blog with ID: {BlogId}", id);
            throw;
        }
    }
}