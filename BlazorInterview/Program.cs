using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorInterview;
using BlazorInterview.Services; 
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register DataService
builder.Services.AddScoped<DataService>();
// Register Bootstrap
builder.Services.AddBlazorBootstrap();
// Register MudBlazor
builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// Register IDataService with a scoped lifetime
builder.Services.AddScoped<IDataService, DataService>();

await builder.Build().RunAsync();
