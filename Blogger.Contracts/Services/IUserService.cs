namespace Blogger.Contracts.Services;

public interface IUserService
{
    public bool IsAuthorized { get; }
    public string UserId { get; }
}