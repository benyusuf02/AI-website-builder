namespace YDeveloper.Models.DTOs
{
    public record UserDto(string Id, string Email, string UserName, bool EmailConfirmed);
    public record CreateUserDto(string Email, string Password, string UserName);
    public record UpdateUserDto(string? Email, string? UserName);
    
    public record LoginDto(string Email, string Password);
    public record RegisterDto(string Email, string Password, string ConfirmPassword, string UserName);
    public record TokenDto(string Token, DateTime ExpiresAt);
}
