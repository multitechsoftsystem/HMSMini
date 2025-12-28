using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HMSMini.Web;
using HMSMini.Web.Services;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Configure HttpClient for API
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5096") // API base URL
    };
    return client;
});

// Register services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IApiClientService, ApiClientService>();

// Initialize authentication on startup
var host = builder.Build();

var authService = host.Services.GetRequiredService<IAuthenticationService>();
var httpClient = host.Services.GetRequiredService<HttpClient>();
var token = await authService.GetTokenAsync();

if (!string.IsNullOrEmpty(token))
{
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}

await host.RunAsync();
