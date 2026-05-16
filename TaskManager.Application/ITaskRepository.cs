using TaskManager.Domain;

namespace TaskManager.Application;

public interface ITaskRepository
{
    IEnumerable<TaskItem> GetAll();
    TaskItem? GetById(Guid id);
    TaskItem Create(CreateTaskRequest request);
    TaskItem? Update(Guid id, UpdateTaskRequest request);
    bool Delete(Guid id);
}
