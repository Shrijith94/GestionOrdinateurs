using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestionOrdinateurs.Client;
using Blazored.SessionStorage;

//test
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddAuthorizationCore();


builder.Services.AddHttpClient<ApiLogin>("HttpClientName", client =>
{
    client.BaseAddress = new Uri("http://localhost:5066/");
});
builder.Services.AddHttpClient<ApiOrdinateurs>(client =>
{
    // client.BaseAddress = new Uri("http://localhost:5075/");
    client.BaseAddress = new Uri("http://localhost:5010/"); // avec le gateway
});
builder.Services.AddHttpClient<ApiReparations>(client =>
{
    // client.BaseAddress = new Uri("http://localhost:5075/"); sans le gateway
    client.BaseAddress = new Uri("http://localhost:5010/"); // avec le gateway
});
await builder.Build().RunAsync();
