using AutoMapper;
using BLL.DTOs.Advertisement;
using BLL.DTOs.Category;
using BLL.DTOs.Tag;
using BLL.DTOs.User;
using DAL.Entities;

namespace BLL;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryResponse>()
            .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories));

        CreateMap<Tag, TagResponse>();

        CreateMap<User, UserResponse>();

        CreateMap<Advertisement, AdvertisementResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()));
    }
}