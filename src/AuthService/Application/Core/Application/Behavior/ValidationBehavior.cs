using FluentValidation;
using MediatR;

namespace Application.Behavior;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"ValidationBehavior invoked for {typeof(TRequest).Name}");
        
        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToList();

        if (errors.Count != 0)
        {
            throw new ValidationException(errors);
        }
        
        return await next(cancellationToken);
    }
}