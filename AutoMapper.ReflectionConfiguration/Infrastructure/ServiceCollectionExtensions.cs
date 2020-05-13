namespace AutoMapper.ReflectionConfiguration.Infrastructure
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            AutoMapperConfig.RegisterMappings(assemblies: assemblies);
            services.AddSingleton<IMapper>(AutoMapperConfig.MapperInstance);

            return services;
        }
    }
}
