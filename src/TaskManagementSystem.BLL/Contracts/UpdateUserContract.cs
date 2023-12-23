using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.BLL.Contracts;

public class UpdateUserContract
{
    [Required]
    public string Name { get; set; }
}