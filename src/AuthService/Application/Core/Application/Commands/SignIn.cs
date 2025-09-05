using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Contracts;
using Application.Contracts.DTO.SignIn;
using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Options;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Commands;

public record SignIn(string Email, string Password) : IRequest<TokenDto>;

public sealed class SignInCommandHandler(UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IOptions<JwtOptions> jwtOptions,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SignIn, TokenDto>
{
    public async Task<TokenDto> Handle(SignIn request, CancellationToken cancellationToken)
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

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var refreshToken = await unitOfWork.RefreshTokenRepository.GetFreshToken(user.Id, cancellationToken);

            if (refreshToken is null)
            {
                var randomNumber = new byte[32];
                using var generator = RandomNumberGenerator.Create();
                generator.GetBytes(randomNumber);
                refreshToken = new RefreshToken
                {
                    User = user,
                    Issued = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(30),
                    Token = Convert.ToBase64String(randomNumber),
                    IpAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
                };
                
                await unitOfWork.RefreshTokenRepository.CreateAsync(refreshToken, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var signingOptions = GenerateTokenOptions(signingCredentials, claims);
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(signingOptions);

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return new TokenDto(accessToken, refreshToken.Token!);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);

            throw;
        }
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        var secret = Environment.GetEnvironmentVariable("SECRET");
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
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtOptions.Value.Expires)),
            signingCredentials: signingCredentials
        );
    }
}

public sealed class SignInCommandValidator : AbstractValidator<SignInRequestDto>
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