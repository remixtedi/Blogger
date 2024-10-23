namespace Blogger.Contracts.Entities;

public class Blog : AuditableEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
}