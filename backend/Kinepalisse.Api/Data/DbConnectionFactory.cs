using System.Data;
using MySqlConnector;

namespace Kinepalisse.Api.Data;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionString 'Default' manquante.");
    }

    // Crée et retourne une connexion ouverte. L'appelant DOIT la fermer (via using).
    public async Task<IDbConnection> CreateOpenConnectionAsync()
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }
}
