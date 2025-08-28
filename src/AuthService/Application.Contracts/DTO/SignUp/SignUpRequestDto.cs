namespace Application.Contracts.DTO.SignUp;

public record SignUpRequestDto(string UserName, string Email, string Password, string ConfirmPassword);