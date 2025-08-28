using Application.Mapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.SignUp;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly MapperRegistry _mapper;
    
    public SignUpCommandHandler(UserManager<User> userManager, MapperRegistry mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
    
    public async Task Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<SignUpCommand, User>(request);

        await _userManager.CreateAsync(user, request.Password);
    }
}