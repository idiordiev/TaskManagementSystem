using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Application.Contracts;

public class UpdateUserContract
{
    [Required]
    public string Name { get; set; }
}