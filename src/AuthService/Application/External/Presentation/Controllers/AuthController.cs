using Application.Commands;
using Application.Contracts.DTO.SignIn;
using Application.Contracts.DTO.SignUp;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    ISender sender,
    IValidator<SignUpRequestDto> signUpValidator,
    IValidator<SignInRequestDto> signInValidator)
    : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpRequestDto requestDto, CancellationToken cancellationToken)
    {
        await signUpValidator.ValidateAndThrowAsync(requestDto, cancellationToken);
        
        var signUpCommand = new SignUp(requestDto);
        
        await sender.Send(signUpCommand, cancellationToken);

        return Ok();
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInRequestDto requestDto, CancellationToken cancellationToken)
    {
        await signInValidator.ValidateAndThrowAsync(requestDto, cancellationToken);

        var signInCommand = new SignIn(requestDto.Email, requestDto.Password);

        var token = await sender.Send(signInCommand, cancellationToken);

        return Ok(token);
    }
}