using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using System;


namespace ChessAndQuests.DAL
{

    public class PlayerMethods
    {
        private SqlConnection sqlconnection;
        private string conString;
        public PlayerMethods() {
            sqlconnection = new SqlConnection();
            conString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";
            sqlconnection.ConnectionString = conString;
        }
        // get all players
        public List<PlayerDetails> GetAll(out string errormsg)
        {
            var players = new List<PlayerDetails>();
            errormsg = "";

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "Select * FROM tbl_player";
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

        
        public PlayerDetails GetById(int playerId)
        {
            PlayerDetails player = null;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT * FROM tbl_player WHERE pl_id = @PlayerId";
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

            using (SqlConnection conn = new SqlConnection(conString))
            {

                string query = "SELECT * FROM player WHERE username = @Username";
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

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = @"INSERT INTO tbl_player (pl_username, pl_password) 
                               VALUES (@Username, @Password)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", player.PlayerUserName);
                cmd.Parameters.AddWithValue("@Password", player.PlayerPassword);

                conn.Open();
                int newId = (int)cmd.ExecuteNonQuery();

                return newId;
            }
        }
        public bool Update(PlayerDetails player)
        {
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = @"UPDATE tbl_player 
                               SET pl_username = @Username, 
                                   pl_password = @Password 
                               WHERE pl_id = @PlayerId";

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
                string query = "DELETE FROM tbl_player WHERE pl_id = @PlayerId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PlayerId", playerId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

    }

}
