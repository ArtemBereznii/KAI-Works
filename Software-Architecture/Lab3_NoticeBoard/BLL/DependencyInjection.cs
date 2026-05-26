using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using BLL.Interfaces;
using BLL.Services;

namespace BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(config => { }, assembly);

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IAdvertisementService, AdvertisementService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}