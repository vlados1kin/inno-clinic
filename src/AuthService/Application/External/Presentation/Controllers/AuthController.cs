using Application.Commands.SignIn;
using Application.Commands.SignUp;
using Application.Contracts.DTO.SignIn;
using Application.Contracts.DTO.SignUp;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ISender _sender;
    private readonly IValidator<SignUpRequestDto> _signUpValidator;
    private readonly IValidator<SignInRequestDto> _signInValidator;
    
    public AuthController(UserManager<User> userManager,
        ISender sender,
        IValidator<SignUpRequestDto> signUpValidator,
        IValidator<SignInRequestDto> signInValidator)
    {
        _userManager = userManager;
        _sender = sender;
        _signUpValidator = signUpValidator;
        _signInValidator = signInValidator;
    }

    /// <summary>
    /// Only for testing proposes
    /// </summary>
    /// <returns></returns>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpRequestDto requestDto, CancellationToken cancellationToken)
    {
        await _signUpValidator.ValidateAndThrowAsync(requestDto, cancellationToken);
        
        var command = new SignUpCommand(requestDto.UserName, requestDto.Email, requestDto.Password, requestDto.ConfirmPassword);
        
        await _sender.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInRequestDto requestDto, CancellationToken cancellationToken)
    {
        await _signInValidator.ValidateAndThrowAsync(requestDto, cancellationToken);

        var signInCommand = new SignInCommand(requestDto.Email, requestDto.Password);

        var token = await _sender.Send(signInCommand, cancellationToken);

        return Ok(token);
    }
}