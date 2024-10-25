namespace Blogger.Contracts.Models.Requests;

public class FilterBlogsRequest : PaginationFilterRequest
{
    public string? Keyword { get; set; }
}