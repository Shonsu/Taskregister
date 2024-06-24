using System.ComponentModel.DataAnnotations;

namespace Taskregister.Server.User.Controllers.Dto;

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}