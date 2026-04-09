using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazingSpire.Demo;
using BlazingSpire.Demo.Components.Demo;
using BlazingSpire.Demo.Components.UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IComponentMetaService, ComponentMetaService>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddSingleton<IDialogService, DialogService>();

await builder.Build().RunAsync();
