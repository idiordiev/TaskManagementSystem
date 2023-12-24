namespace TaskManagementSystem.Application.Exceptions;

public class UserExistsException : Exception
{
    public UserExistsException()
    {
    }

    public UserExistsException(string message) : base(message)
    {
    }
}