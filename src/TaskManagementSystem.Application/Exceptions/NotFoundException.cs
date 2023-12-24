namespace TaskManagementSystem.Application.Exceptions;

[Serializable]
public class NotFoundException : ApplicationException
{
    public NotFoundException(string entityName, int id) : base($"The entity {entityName} with id {id} has not been found")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}