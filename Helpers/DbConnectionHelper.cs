// Helpers/DbConnectionHelper.cs
using MySqlConnector;

namespace LoanBack.Helpers
{
    public static class DbConnectionHelper
    {
        public static string BuildServerConnection(string fullConnectionString)
        {
            var builder = new MySqlConnectionStringBuilder(fullConnectionString)
            {
                Database = "" // removes the DB name
            };
            return builder.ConnectionString;
        }

        public static string BuildDatabaseConnection(string fullConnectionString, string dbName)
        {
            var builder = new MySqlConnectionStringBuilder(fullConnectionString)
            {
                Database = dbName
            };
            return builder.ConnectionString;
        }

        public static (string ServerConn, string DbConn) BuildBoth(string fullConnectionString, string dbName)
        {
            var serverConn = BuildServerConnection(fullConnectionString);
            var dbConn = BuildDatabaseConnection(fullConnectionString, dbName);
            return (serverConn, dbConn);
        }
    }
}

