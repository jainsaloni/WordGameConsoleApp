using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WordGameConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString;
            connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            Console.Clear();

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            PlayGame game = new PlayGame();
            game.GetPlayer(sqlConnection);

            sqlConnection.Close();
        }
    }
}
