using Blogger.Contracts.Enums;
using Blogger.Contracts.Models;
using Blogger.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class BlogsController(ILogger<BlogsController> logger, IBlogsService blogsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var blogs = await blogsService.GetBlogsAsync(e=> e.EntityStatus == EntityStatus.Active);
            return Ok(blogs);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all blogs");
            return StatusCode(500, "An error occurred while retrieving blogs");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var blog = await blogsService.GetBlogAsync(id);
            return Ok(blog);
        }
        catch (Exception ex)
        {
            if (ex.Message == "Blog not found")
            {
                logger.LogWarning("Blog not found with ID: {BlogId}", id);
                return NotFound($"Blog with ID {id} not found");
            }
            
            logger.LogError(ex, "Error retrieving blog with ID: {BlogId}", id);
            return StatusCode(500, "An error occurred while retrieving the blog");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BlogDTO blog)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid blog creation request: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            var createdBlog = await blogsService.CreateBlogAsync(blog);
            
            logger.LogInformation("Created new blog with ID: {BlogId}", createdBlog.Id);
            return CreatedAtAction(nameof(GetById), new { id = createdBlog.Id }, createdBlog);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating new blog");
            return StatusCode(500, "An error occurred while creating the blog");
        }
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BlogDTO blog)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid blog update request for ID {BlogId}: {Errors}", 
                    id, string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            var updatedBlog = await blogsService.UpdateBlogAsync(id, blog);
            
            logger.LogInformation("Updated blog: {BlogId}", id);
            return Ok(updatedBlog);
        }
        catch (Exception ex)
        {
            if (ex.Message == "Blog not found")
            {
                logger.LogWarning("Blog not found for update with ID: {BlogId}", id);
                return NotFound($"Blog with ID {id} not found");
            }
            
            logger.LogError(ex, "Error updating blog with ID: {BlogId}", id);
            return StatusCode(500, "An error occurred while updating the blog");
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await blogsService.SoftDeleteBlogAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex.Message == "Blog not found")
            {
                logger.LogWarning("Blog not found for deletion with ID: {BlogId}", id);
                return NotFound($"Blog with ID {id} not found");
            }
            
            logger.LogError(ex, "Error deleting blog with ID: {BlogId}", id);
            return StatusCode(500, "An error occurred while deleting the blog");
        }
    }
}