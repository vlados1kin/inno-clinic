using Application.Commands;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.ConfigureMediatr();
        services.ConfigureValidation();
        
        return services;
    }

    private static IServiceCollection ConfigureMediatr(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<SignUp>());

        return services;
    }

    private static IServiceCollection ConfigureValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<SignUpCommandValidator>(includeInternalTypes: true);

        return services;
    }
}