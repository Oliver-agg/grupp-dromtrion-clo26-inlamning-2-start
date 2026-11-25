using Microsoft.Extensions.Configuration;

namespace Recept.Data;

// Hämtar connectionstringen så att den aldrig behöver stå i koden.
// Den letar först i user-secrets och sedan bland miljövariabler,
// så att läraren kan sätta sin egen sträng på sin dator.
public static class ConnectionStringProvider
{
    private const string ConnectionKey = "DB_CONNECTION";

    public static string GetConnectionString()
    {   
        IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<RecipeConnectionMarker>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        string? connectionString = configuration[ConnectionKey];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Hittar ingen connectionstring. Kör: " +
                "dotnet user-secrets set 'DB_CONNECTION' '<din connectionstring>' --project Recept-Konsol");
        }

        return connectionString;
    }
}

// Tom klass som bara visar vilket projekt user-secrets ska läsas från.
internal sealed class RecipeConnectionMarker { }
