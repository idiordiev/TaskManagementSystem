using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Application.Contracts;

public class CreateUserContract
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(10)]
    [MaxLength(64)]
    public string Password { get; set; }
}