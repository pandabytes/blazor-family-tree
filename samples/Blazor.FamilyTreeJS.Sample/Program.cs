using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor.FamilyTreeJS.Sample;

using Blazor.FamilyTreeJS;
using Blazor.Core;

using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
  .AddBlazorFamilyJS()
  .AddScoped<DialogService>()
  .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var webHost = builder
  .Build()
  .ConfigureIJSRuntimeJsonOptions();

await webHost.Services.RegisterAttachReviverAsync();
await webHost.RunAsync();