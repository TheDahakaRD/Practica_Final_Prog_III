using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;

public class TaskService
{
    private readonly ILocalStorageService _localStorageService;

    private const string TaskKey = "tasks";

    public TaskService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    // Obtener tareas del localStorage
    public async Task<List<Task>> GetTasks()
    {
        var serializedTasks = await _localStorageService.GetItemAsync<string>(TaskKey);
        if (serializedTasks == null)
        {
            return new List<Task>();
        }

        return System.Text.Json.JsonSerializer.Deserialize<List<Task>>(serializedTasks);
    }

    // Agregar una tarea al localStorage
    public async Task AddTask(Task newTask)
    {
        var tasks = await GetTasks();
        tasks.Add(newTask);
        await SaveTasks(tasks);
    }


    // Eliminar una tarea del localStorage
    public async Task DeleteTask(int id)
    {
        var tasks = await GetTasks();
        var taskToRemove = tasks.FirstOrDefault(t => t.Id == id);

        if (taskToRemove != null)
        {
            tasks.Remove(taskToRemove);
            await SaveTasks(tasks);
        }
    }

    private async Task SaveTasks(List<Task> tasks)
    {
        var serializedTasks = System.Text.Json.JsonSerializer.Serialize(tasks);
        await _localStorageService.SetItemAsync(TaskKey, serializedTasks);
    }
}
