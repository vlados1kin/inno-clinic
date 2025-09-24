using Application.Abstractions;
using Application.Behavior;
using Application.Commands;
using Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.ConfigureValidation();
        services.ConfigureMediatr();
        services.ConfigureServices();
        
        return services;
    }

    private static IServiceCollection ConfigureMediatr(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SignUpCommand>());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }

    private static IServiceCollection ConfigureValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<SignUpCommandValidator>(includeInternalTypes: true);

        return services;
    }

    private static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        return services;
    }
}