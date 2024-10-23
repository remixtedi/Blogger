using Microsoft.AspNetCore.Mvc;

namespace Blogger.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BlogsController(ILogger<BlogsController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Update()
    {
        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        return Ok();
    }
}