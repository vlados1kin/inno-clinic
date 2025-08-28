using System.Text.RegularExpressions;
using Application.Contracts.DTO.SignUp;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Application.Validators;

internal sealed class SignUpCommandValidator : AbstractValidator<SignUpRequestDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IdentityOptions _identityOptions;
    
    public SignUpCommandValidator(UserManager<User> userManager, IOptions<IdentityOptions> identityOptions)
    {
        _userManager = userManager;
        _identityOptions = identityOptions.Value;

        RuleFor(command => command.UserName)
            .NotEmpty()
            .WithMessage("Please, enter the user name.")
            .Must(userName => userName.All(letter => _identityOptions.User.AllowedUserNameCharacters.Contains(letter)))
            .WithMessage("Username contains invalid characters. Available symbols: a-z A-Z 0-9 - . _")
            .MustAsync(HasUniqueUserName)
            .WithMessage("User with this user name already exists.");
        
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Please, enter the email.")
            .EmailAddress()
            .WithMessage("You've entered an invalid email.")
            .MustAsync(HasUniqueEmail)
            .WithMessage("User with this email already exists.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Please, enter the password.")
            .MinimumLength(_identityOptions.Password.RequiredLength)
            .WithMessage($"The minimum length of a password is {_identityOptions.Password.RequiredLength}.")
            .Must(password => !_identityOptions.Password.RequireDigit || Regex.IsMatch(password, @"\d"))
            .WithMessage("Password must contain at least one digit.")
            .Must(password => !_identityOptions.Password.RequireLowercase || Regex.IsMatch(password, "[a-z]"))
            .WithMessage("Password must contain at least one lowercase letter.")
            .Must(password => !_identityOptions.Password.RequireUppercase || Regex.IsMatch(password, "[A-Z]"))
            .WithMessage("Password must contain at least one uppercase letter.")
            .Must(password => !_identityOptions.Password.RequireNonAlphanumeric || Regex.IsMatch(password, @"\W"))
            .WithMessage("Password must contain at least one non-alphanumeric character.");

        RuleFor(command => command.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Please, re-enter the password.");

        RuleFor(common => common)
            .Must(command => command.Password == command.ConfirmPassword)
            .WithMessage("The passwords you’ve entered don’t coincide.");
    }

    private async Task<bool> HasUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return !_identityOptions.User.RequireUniqueEmail || user == null;
    }

    private async Task<bool> HasUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user == null;
    }
}