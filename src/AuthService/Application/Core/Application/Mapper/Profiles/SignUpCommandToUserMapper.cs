using Application.Commands.SignUp;
using Application.Contracts.Mapper;
using Domain.Entities;

namespace Application.Mapper.Profiles;

public sealed class SignUpCommandToUserMapper : IMapper<SignUpCommand, User>
{
    public User Map(SignUpCommand model)
    {
        return new User
        {
            UserName = model.UserName,
            NormalizedUserName = model.UserName.Normalize(),
            Email = model.Email,
            NormalizedEmail = model.Email.Normalize()
        };
    }
}