using System.ComponentModel.DataAnnotations;

namespace Blogger.Contracts.Models;

public class BlogDTO
{
    public int Id { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Name length can't be less than 8 characters")]
    public string Title { get; set; }

    [Required]
    [MinLength(30, ErrorMessage = "Content length can't be less than 8 characters")]
    public string Content { get; set; }
}