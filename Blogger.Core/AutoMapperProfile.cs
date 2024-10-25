using AutoMapper;
using Blogger.Contracts.Data.Entities;
using Blogger.Contracts.Models;

namespace Blogger.Core;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<BlogDTO, Blog>();
        CreateMap<Blog, BlogDTO>();
    }
}