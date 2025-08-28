using Application.Commands.SignUp;
using Application.Mapper;
using Application.Mapper.Profiles;
using Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.ConfigureMapper();
        services.ConfigureMediatr();
        services.ConfigureValidation();
        
        return services;
    }

    private static IServiceCollection ConfigureMapper(this IServiceCollection services)
    {
        services.AddSingleton<MapperRegistry>(_ =>
        {
            var mapper = new MapperRegistry();
            mapper.Registry(new SignUpCommandToUserMapper());
            
            return mapper;
        });

        return services;
    }

    private static IServiceCollection ConfigureMediatr(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<SignUpCommand>());

        return services;
    }

    private static IServiceCollection ConfigureValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<SignUpCommandValidator>(includeInternalTypes: true);

        return services;
    }
}