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

// Register message handler
builder.Services.AddScoped<AuthorizationMessageHandler>();

// Configure HttpClient for API with authorization handler
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();

    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5096/")
    };

    return client;
});

// Register services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IApiClientService, ApiClientService>();

await builder.Build().RunAsync();
