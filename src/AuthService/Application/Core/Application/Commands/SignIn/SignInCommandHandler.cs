using Application.Contracts.DTO.SignIn;
using Application.Contracts.Exceptions;
using Domain.Entities;
using Domain.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Application.Commands.SignIn;

public sealed class SignInCommandHandler : IRequestHandler<SignInCommand, TokenDto>
{
    private readonly UserManager<User> _userManager;
    private readonly JwtOptions _jwtOptions;

    public SignInCommandHandler(UserManager<User> userManager, IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<TokenDto> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new UnauthorizedException("Either an email or a password is incorrect");
        
        var passwordIsMatched = await _userManager.CheckPasswordAsync(user, request.Password);
        
        if (!passwordIsMatched)
            throw new UnauthorizedException("Either an email or a password is incorrect");

        // TODO: Implement token generation logic
        return new TokenDto(string.Empty, string.Empty);
    }
}