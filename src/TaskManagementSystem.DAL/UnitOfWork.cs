using TaskManagementSystem.DAL.Interfaces;
using TaskManagementSystem.DAL.Repositories;

namespace TaskManagementSystem.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    private ITaskRepository? _taskRepository;
    private ISubtaskRepository? _subtaskRepository;
    private IUserRepository? _userRepository;

    public UnitOfWork(DataContext context)
    {
        _context = context;
    }

    public ITaskRepository TaskRepository
    {
        get
        {
            if (_taskRepository is null)
            {
                _taskRepository = new TaskRepository(_context);
            }

            return _taskRepository;
        }
    }

    public ISubtaskRepository SubtaskRepository
    {
        get
        {
            if (_subtaskRepository is null)
            {
                _subtaskRepository = new SubtaskRepository(_context);
            }

            return _subtaskRepository;
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            if (_userRepository is null)
            {
                _userRepository = new UserRepository(_context);
            }

            return _userRepository;
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}