using Domain.Entities;

namespace Application.Contracts.DTO.SignUp;

public record SignUpRequestDto(string UserName, string Email, string Password, string ConfirmPassword)
{
    public User ToUser()
    {
        return new User
        {
            UserName = UserName,
            NormalizedUserName = UserName.Normalize(),
            Email = Email,
            NormalizedEmail = Email.Normalize()
        };
    }
}