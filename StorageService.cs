using Microsoft.JSInterop;  
using System.Text.Json;
using System.Threading.Tasks;
using Let_s_Do_It.Models; 
public class StorageService
{
    private readonly IJSRuntime _jsRuntime;

    public StorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }


    public async Task SaveTasks(List<TaskModel> tasks)
    {
        var jsonData = JsonSerializer.Serialize(tasks);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "tasks", jsonData);
    }

    public async Task<List<TaskModel>> GetTasks()
    {
        var jsonData = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "tasks");
        return jsonData == null ? new List<TaskModel>() : JsonSerializer.Deserialize<List<TaskModel>>(jsonData);
    }

    public async Task RemoveFromLocalStorage(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}
