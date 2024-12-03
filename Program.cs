using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using let_s_Do_It;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<StorageService>();
builder.Services.AddSingleton<TaskService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();


public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class TaskModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
}

