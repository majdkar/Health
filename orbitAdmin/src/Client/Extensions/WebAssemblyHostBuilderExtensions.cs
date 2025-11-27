using Blazored.LocalStorage;
using SchoolV01.Client.Infrastructure.Authentication;
using SchoolV01.Client.Infrastructure.Managers;
using SchoolV01.Client.Infrastructure.Managers.Preferences;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using SchoolV01.Client.Infrastructure.Managers.ExtendedAttribute;
using SchoolV01.Domain.Entities.ExtendedAttributes;
using SchoolV01.Domain.Entities.Misc;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using SchoolV01.Shared.Constants.Role;
using SchoolV01.Client.Infrastructure.Settings;

namespace SchoolV01.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        private const string ClientName = "BlazorHero.API";

        public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#app");

            return builder;
        }

        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
            builder
                .Services
                .AddLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                })
                .AddAuthorizationCore(RegisterPermissionClaims)
                .AddBlazoredLocalStorage()
                .AddMudServices(config =>
                {
                    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                    config.SnackbarConfiguration.HideTransitionDuration = 500;
                    config.SnackbarConfiguration.ShowTransitionDuration = 500;
                    config.SnackbarConfiguration.VisibleStateDuration = 3000;
                    config.SnackbarConfiguration.ShowCloseIcon = true;
                    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                })
                .AddAutoMapper(cfg =>
                {
                    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
                })         
                 .AddScoped<ClientPreferenceManager>()
                .AddScoped<BlazorHeroStateProvider>()
                .AddScoped<AuthenticationStateProvider, BlazorHeroStateProvider>()
                .AddManagers()
                .AddExtendedAttributeManagers()
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClientInterceptor();
            return builder;
        }

        public static IServiceCollection AddManagers(this IServiceCollection services)
        {
            var managers = typeof(IManager);

            var types = managers
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (managers.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }

        public static IServiceCollection AddExtendedAttributeManagers(this IServiceCollection services)
        {
            //TODO - add managers with reflection!

            return services
                .AddTransient(typeof(IExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>), typeof(ExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>));
        }

        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    //options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireAssertion(context => AdminOrCan(context, propertyValue.ToString())));
                }
            }
        }

        public static bool AdminOrCan(AuthorizationHandlerContext context, string claim)
        {
            bool isAdmin = context.User.IsInRole(RoleConstants.AdministratorRole);
            bool can = context.User.HasClaim(ApplicationClaimTypes.Permission, claim);

            return isAdmin || can;
        }
        public static CultureInfo GetGeneralCulture(this CultureInfo culture)
        {
            culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

            culture.NumberFormat.NumberDecimalDigits = 2;
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
            culture.NumberFormat.NumberNegativePattern = 0;

            if (culture.Name.Contains("ar"))
            {
                string[] monthNames = ["كانون الثاني", "شباط", "آذار", "نيسان", "أيار", "حزيران", "تموز", "آب", "ايلول", "تشرين الأول", "تشرين الثاني", "كانون الأول",""];
                culture.DateTimeFormat.AbbreviatedMonthNames =
                    culture.DateTimeFormat.MonthNames =
                    culture.DateTimeFormat.MonthGenitiveNames =
                    culture.DateTimeFormat.AbbreviatedMonthGenitiveNames = monthNames;

            }

            return culture;
        }

    }
}