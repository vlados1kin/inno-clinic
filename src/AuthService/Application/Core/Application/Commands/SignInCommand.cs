using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions;
using Application.DTO;
using Domain;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Options;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Commands;

public sealed record SignInCommand(string Email, string Password) : IRequest<TokenDto>;

public sealed class SignInCommandHandler(
    UserManager<User> userManager,
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtOptions> jwtOptions,
    IConfiguration configuration) : IRequestHandler<SignInCommand, TokenDto>
{
    public async Task<TokenDto> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new UnauthorizedException("Either an email or a password is incorrect");
        }

        var passwordIsMatched = await userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordIsMatched)
        {
            throw new UnauthorizedException("Either an email or a password is incorrect");
        }


        var refreshToken = await refreshTokenRepository.GetFreshTokenAsync(user.Id, cancellationToken) ??
                           await refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);

        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims(user);
        var signingOptions = GenerateTokenOptions(signingCredentials, claims);
        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(signingOptions);
        
        return new TokenDto(accessToken, refreshToken.Token!);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = configuration["JwtOptions:Secret"];

        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException("Jwt secret is no configured.");
        }
        
        var bytes = Encoding.UTF8.GetBytes(secret!);
        var key = new SymmetricSecurityKey(bytes);

        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims(User user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        return new JwtSecurityToken
        (
            issuer: jwtOptions.Value.ValidIssuer,
            audience: jwtOptions.Value.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtOptions.Value.AccessTokenExpires)),
            signingCredentials: signingCredentials
        );
    }
}

public sealed class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Please, enter the email.")
            .EmailAddress()
            .WithMessage("You've entered an invalid email.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Please, enter the password.");
    }
}