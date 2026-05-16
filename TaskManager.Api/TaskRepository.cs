namespace TaskManager.Api;

public class TaskRepository
{
    private readonly Dictionary<Guid, TaskItem> _tasks = [];

    public IEnumerable<TaskItem> GetAll() => _tasks.Values;

    public TaskItem? GetById(Guid id) =>
        _tasks.TryGetValue(id, out var task) ? task : null;

    public TaskItem Create(CreateTaskRequest request)
    {
        var task = new TaskItem { Title = request.Title, Description = request.Description };
        _tasks[task.Id] = task;
        return task;
    }

    public TaskItem? Update(Guid id, UpdateTaskRequest request)
    {
        if (!_tasks.TryGetValue(id, out var task)) return null;
        task.Title = request.Title;
        task.Description = request.Description;
        task.IsComplete = request.IsComplete;
        return task;
    }

    public bool Delete(Guid id) => _tasks.Remove(id);
}
