using System.Text.RegularExpressions;
using Domain;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Commands;

public sealed record SignUpCommand(string UserName, string Email, string Password, string ConfirmPassword) : IRequest
{
    public User ToUser()
    {
        return new User
        {
            UserName = UserName,
            NormalizedUserName = UserName.Normalize(),
            Email = Email,
            NormalizedEmail = Email.Normalize()
        };
    }
}

public sealed class SignUpCommandHandler(
    UserManager<User> userManager,
    IOptions<IdentityOptions> identityOptions) : IRequestHandler<SignUpCommand>
{
    public async Task Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        if (!identityOptions.Value.User.RequireUniqueEmail || await userManager.Users.AnyAsync(user => user.Email == request.Email, cancellationToken))
        {
            throw new AlreadyExistsException("User with this email already exists.");
        }

        if (await userManager.Users.AnyAsync(user => user.UserName == request.UserName, cancellationToken))
        {
            throw new AlreadyExistsException("User with this user name already exists.");
        }
        
        var user = request.ToUser();

        await userManager.CreateAsync(user, request.Password);
    }
}

public sealed class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator(IOptions<IdentityOptions> identityOptions)
    {
        RuleFor(command => command.UserName)
            .NotEmpty()
            .WithMessage("Please, enter the user name.")
            .Must(userName => userName.All(letter => identityOptions.Value.User.AllowedUserNameCharacters.Contains(letter)))
            .WithMessage("Username contains invalid characters. Available symbols: a-z A-Z 0-9 - . _");
        
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Please, enter the email.")
            .EmailAddress()
            .WithMessage("You've entered an invalid email.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Please, enter the password.")
            .MinimumLength(identityOptions.Value.Password.RequiredLength)
            .WithMessage($"The minimum length of a password is {identityOptions.Value.Password.RequiredLength}.")
            .Must(password => !identityOptions.Value.Password.RequireDigit || Regex.IsMatch(password, @"\d"))
            .WithMessage("Password must contain at least one digit.")
            .Must(password => !identityOptions.Value.Password.RequireLowercase || Regex.IsMatch(password, "[a-z]"))
            .WithMessage("Password must contain at least one lowercase letter.")
            .Must(password => !identityOptions.Value.Password.RequireUppercase || Regex.IsMatch(password, "[A-Z]"))
            .WithMessage("Password must contain at least one uppercase letter.")
            .Must(password => !identityOptions.Value.Password.RequireNonAlphanumeric || Regex.IsMatch(password, @"\W"))
            .WithMessage("Password must contain at least one non-alphanumeric character.");

        RuleFor(command => command.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Please, re-enter the password.");

        RuleFor(common => common)
            .Must(command => command.Password == command.ConfirmPassword)
            .WithMessage("The passwords you’ve entered don’t coincide.");
    }
}