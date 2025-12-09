using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using System;

namespace ChessAndQuests.DAL
{

    public class PlayerMethods
    {
        // get all players
        public List<PlayerDetails> GetAll(out string errormsg)
        {
            var players = new List<PlayerDetails>();
            errormsg = "";
            string connectionString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "Select player_id, username, password FROM player";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    players.Add(new PlayerDetails()
                    {
                        PlayerId = reader.GetInt32(0),
                        PlayerUserName = reader.GetString(1),
                        PlayerPassword = reader.GetString(2)

                    });


                }

            }
            return players;

        }

        // get player by Id
        public PlayerDetails GetById(int playerId)
        {
            PlayerDetails player = null;

            string connectionString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT player_id, username, password FROM player WHERE player_id = @PlayerId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PlayerId", playerId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    player = new PlayerDetails
                    {
                        PlayerId = reader.GetInt32(0),
                        PlayerUserName = reader.GetString(1),
                        PlayerPassword = reader.GetString(2)
                    };
                }


            }
            return player;

        }
        public PlayerDetails GetByUserName(string username)
        {
            PlayerDetails player = null;

            string connectionString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                string query = "Slect player_id,username, password FROM player WHERE username = @Username";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Username", username);
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    player = new PlayerDetails
                    {
                        PlayerId = reader.GetInt32(0),
                        PlayerUserName = reader.GetString(1),
                        PlayerPassword = reader.GetString(2)

                    };
                }
            }

            return player;
        }

        public int InsertUser(PlayerDetails player)
        {
            string connectionString = "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO player (username, password) 
                               VALUES (@Username, @Password);
                               SELECT CAST(SCOPE_IDENTITY() as int)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", player.PlayerUserName);
                cmd.Parameters.AddWithValue("@Password", player.PlayerPassword);

                conn.Open();
                int newId = (int)cmd.ExecuteScalar();

                return newId;
            }
        }
        public bool Update(PlayerDetails player)
        {
            string connectionString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE player 
                               SET username = @Username, 
                                   password = @Password 
                               WHERE player_id = @PlayerId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PlayerId", player.PlayerId);
                cmd.Parameters.AddWithValue("@Username", player.PlayerUserName);
                cmd.Parameters.AddWithValue("@Password", player.PlayerPassword);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public bool Delete(int playerId)
        {
            string connectionString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM player WHERE player_id = @PlayerId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PlayerId", playerId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

    }

}
