// See https://aka.ms/new-console-template for more information
using database;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;
using System;

namespace database
{
    internal class Program
    {
        public static DatabaseManager databaseManager = new DatabaseManager();

        static void Main(string[] args)
        {
            PreparedGet();
        }

        static void Get()
        {
            // Simple get example
            SqlDataReader request = databaseManager.Get("SELECT * FROM Joueur");

            while (request.Read())
            {
                Console.WriteLine("User id: " + request.GetValue(0));
                Console.WriteLine("Nom: " + request.GetValue(1));
                Console.WriteLine("Prenom: " + request.GetValue(2));
            }
        }

        static void PreparedGet()
        {
            // Prepared GET example
            SqlParameter param1 = new SqlParameter("@param1", SqlDbType.Int);
            param1.Value = 15;

            SqlDataReader request2 = databaseManager.PreparedGet("SELECT * FROM Joueur WHERE id_joueur = @param1", param1);

            if (request2.Read())
            {
                Console.WriteLine("User id: " + request2.GetValue(0));
                Console.WriteLine("Nom: " + request2.GetValue(1));
                Console.WriteLine("Prenom: " + request2.GetValue(2));
            }
        }
    }
}