
using MySqlConnector;

public class DatabaseBootstrapper
{
    public static void EnsureDatabase(string serverConn, string dbName)
    {
        var builder = new MySqlConnectionStringBuilder(serverConn) { Database = "" };
        using var conn = new MySqlConnection(builder.ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{dbName}`;";
        cmd.ExecuteNonQuery();
    }
}

