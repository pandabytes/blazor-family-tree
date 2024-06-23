using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor.FamilyTreeJS.Sample;

using Blazor.FamilyTreeJS;
using Blazor.Core;

using Radzen;
using Blazor.FamilyTreeJS.Components.Interop.Modules.FamilyTreeStatic;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
  .AddBlazorFamilyJS()
  .AddSingleton<FamilyTreeStaticModule>()
  .AddScoped<DialogService>()
  .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var webHost = builder
  .Build()
  .ConfigureIJSRuntimeJsonOptions();

var familyTreeStaticModule = webHost.Services.GetRequiredService<FamilyTreeStaticModule>();
await familyTreeStaticModule.ImportAsync();

await webHost.Services.RegisterAttachReviverAsync();
await webHost.RunAsync();
