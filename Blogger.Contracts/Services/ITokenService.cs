using Blogger.Contracts.Data.Entities;

namespace Blogger.Contracts.Services;

public interface ITokenService
{
    Task<string> GenerateAsync(ApplicationUser applicationUser);
}