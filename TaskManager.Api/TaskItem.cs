namespace TaskManager.Api;

public class TaskItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public record CreateTaskRequest(string Title, string? Description);
public record UpdateTaskRequest(string Title, string? Description, bool IsComplete);
