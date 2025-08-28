using Application.Contracts.DTO.SignUp;
using MediatR;

namespace Application.Commands.SignUp;

public record SignUpCommand(string UserName,string Email, string Password, string ConfirmPassword) : IRequest;