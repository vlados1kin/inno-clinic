using System.Text.RegularExpressions;
using Application.Commands.SignIn;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Validators;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator(IdentityOptions identityOptions)
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Please, enter the email.")
            .EmailAddress()
            .WithMessage("You've entered an invalid email.");
        
        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Please, enter the password.")
            .MinimumLength(identityOptions.Password.RequiredLength)
            .WithMessage($"The minimum length of a password is {identityOptions.Password.RequiredLength}.")
            .Must(password => !identityOptions.Password.RequireDigit || Regex.IsMatch(password, @"\d"))
            .WithMessage("Password must contain at least one digit.")
            .Must(password => !identityOptions.Password.RequireLowercase || Regex.IsMatch(password, "[a-z]"))
            .WithMessage("Password must contain at least one lowercase letter.")
            .Must(password => !identityOptions.Password.RequireUppercase || Regex.IsMatch(password, "[A-Z]"))
            .WithMessage("Password must contain at least one uppercase letter.")
            .Must(password => !identityOptions.Password.RequireNonAlphanumeric || Regex.IsMatch(password, @"\W"))
            .WithMessage("Password must contain at least one non-alphanumeric character.");
    }
}