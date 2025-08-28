using Application.Contracts.DTO.SignIn;
using MediatR;

namespace Application.Commands.SignIn;

public record GenerateAccessTokenCommand : IRequest<TokenDto>;