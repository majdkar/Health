using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace SchoolV01.Client.Extensions
{
    public static class ConfigurationExtensions
    {
        public static Dictionary<string, string> GetCulturesSection(this IConfiguration configuration)
             => configuration.GetSection("Cultures")
                             .GetChildren()
                             .ToDictionary(k => k.Key, v => v.Value);
    }
}
