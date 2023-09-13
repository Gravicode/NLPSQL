

namespace SchemaGenerator;

using System.Reflection;
using Microsoft.Extensions.Configuration;

internal static class Generator
{
    public static IConfiguration Configuration { get; } = CreateConfiguration();

    private static IConfiguration CreateConfiguration()
    {
        return
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        
    }
}
