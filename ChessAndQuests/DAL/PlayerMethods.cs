using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using System;

namespace ChessAndQuests.DAL
{

    public class PlayerMethods
    {
        private readonly string conString; 
        public PlayerMethods() {
            conString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";
        }
        // get all players
        public List<PlayerDetails> GetAll(out string errormsg)
        {
            var players = new List<PlayerDetails>();
            errormsg = "";

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "Select * FROM player";
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

            using (SqlConnection conn = new SqlConnection(conString))
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


        public int InsertUser(PlayerDetails player)
        {
            using (SqlConnection conn = new SqlConnection(conString))
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
            using (SqlConnection conn = new SqlConnection(conString))
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
            using (SqlConnection conn = new SqlConnection(conString))
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
