namespace Blogger.Contracts.Models.Responses;

public class PaginationFilterResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool ShowPrevious { get; set; }
    public bool ShowNext { get; set; }
}