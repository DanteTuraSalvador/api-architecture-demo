using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DemoApi.BlazorClient;
using DemoApi.BlazorClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure API base address - defaults to Gateway when running together
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:5001";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

// Register API service
builder.Services.AddScoped<ApiService>();

// Register SignalR tracking service as singleton for connection persistence
builder.Services.AddSingleton(sp => new TrackingHubService($"{apiBaseAddress}/hubs/tracking"));

await builder.Build().RunAsync();
