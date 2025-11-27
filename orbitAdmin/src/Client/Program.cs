using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using SchoolV01.Client;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Infrastructure.Managers.Preferences;
using SchoolV01.Client.Infrastructure.Settings;
using SchoolV01.Shared.Constants.Localization;
using System;
using System.Globalization;

var builder = WebAssemblyHostBuilder
              .CreateDefault(args)
              .AddRootComponents()
              .AddClientServices();
//builder.Services.AddScoped<IPrintingService, PrintingService>();

var host = builder.Build();
var storageService = host.Services.GetRequiredService<ClientPreferenceManager>();
if (storageService != null)
{
    CultureInfo culture;
    var preference = await storageService.GetPreference() as ClientPreference;
    if (preference != null)
        culture = new CultureInfo(preference.LanguageCode);
    else
        culture = LocalizationConstants.DefaultCulture;

    culture.GetGeneralCulture();

    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}
// Change the default appearance of all MudButton components
//MudGlobal.InputDefaults.Variant = Variant.Outlined;
// Send all exceptions to the console
MudGlobal.UnhandledExceptionHandler = Console.WriteLine;

await host.RunAsync();

//var builder = WebAssemblyHostBuilder.CreateDefault(args);
//builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//await builder.Build().RunAsync();
