namespace Blogger.Contracts.Models.Responses;

public class FilterBlogsResult : PaginationFilterResponse
{
    public IEnumerable<BlogDTO> Blogs { get; set; }
}