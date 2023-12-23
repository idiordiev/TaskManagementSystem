namespace TaskManagementSystem.BLL.Models;

public class Notification
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public TimeSpan ExpiresIn { get; set; }
    public string Message { get; set; }
}