using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Ilmhub.Spaces.Client;
using MudBlazor.Services;
using Ilmhub.Spaces.Client.Interfaces;
using Ilmhub.Spaces.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<ILeadService, MockLeadService>();
builder.Services.AddScoped<ICourseService, MockCourseService>();

await builder.Build().RunAsync();
