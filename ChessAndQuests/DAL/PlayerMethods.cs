using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;


namespace ChessAndQuests.DAL
{

    public class PlayerMethods
    {
        private SqlConnection sqlConnection;
        private string conString;
        public PlayerMethods() {
            sqlConnection = new SqlConnection();
            conString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";
            sqlConnection.ConnectionString = conString;
        }
        // get all players
        public List<PlayerDetails> GetAllPlayers(out string errormsg)
        {
            string sqlString = "SELECT * FROM tbl_players";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);

            SqlDataReader reader = null;
            List<PlayerDetails> playerDetailsList = new List<PlayerDetails>();

            try
            {
                // Fyll dataset och mappa rader till modeller
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();

                if (!reader.HasRows)
                {
                    errormsg = "No players found";
                    return null;
                }
                while (reader.Read())
                {
                    PlayerDetails playerDetails = new PlayerDetails();
                    playerDetails.PlayerId = Convert.ToUInt16(reader["pl_Id"]);
                    playerDetails.PlayerUserName = Convert.ToString(reader["pl_username"]);
                    playerDetails.PlayerPassword = Convert.ToString(reader["pl_password"]);

                    playerDetailsList.Add(playerDetails);
                }
                errormsg = "";
                return playerDetailsList;

            }
            catch (Exception e)
            {
                // Returnera felmeddelande för att visa vad som gick fel
                errormsg = e.Message;
                return null;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();

                    sqlConnection.Close();
            }
        }

        // Select player by id
        public PlayerDetails GetById(int playerId, out string errormsg)
        {
            SqlDataReader reader = null;
            PlayerDetails player = new PlayerDetails();
            string sqlString = "SELECT * FROM tbl_player WHERE pl_id = @PlayerId";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerId);

            try
            {
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();

                if (!reader.HasRows)
                {
                    errormsg = "No player found with given ID";
                    return null;
                }

                if (reader.Read())
                {

                    player.PlayerId = Convert.ToInt32(reader["pl_id"]);
                    player.PlayerUserName = Convert.ToString(reader["pl_username"]);
                    player.PlayerPassword = Convert.ToString(reader["pl_password"]);
                    
                }
                errormsg = "";
                return player;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();

                    sqlConnection.Close();
            }
        }

        // Add a new player
        public int CreatePlayer(PlayerDetails player, out string errormsg)
        {

            string sqlString = @"INSERT INTO tbl_player (pl_username, pl_password)
                VALUES (@Username, @Password);";

            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Username", player.PlayerUserName);
            sqlCommand.Parameters.AddWithValue("@Password", player.PlayerPassword);


            try
            {
                sqlConnection.Open();
                int i = 0;
                i = sqlCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "Insert failed"; }
                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                sqlConnection.Close();
            }
        }




        // Update an existing player
        public int UpdatePlayer(PlayerDetails player, out string errormsg)
        {

            string sqlString = @"UPDATE tbl_player SET pl_username = @Username, 
                pl_password = @Password WHERE pl_id = @PlayerId;";

            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@PlayerId", player.PlayerId);
            sqlCommand.Parameters.AddWithValue("@Username", player.PlayerUserName);
            sqlCommand.Parameters.AddWithValue("@Password", player.PlayerPassword);

            try
            {
                sqlConnection.Open();
                int i = 0;
                i = sqlCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "Update failed"; }
                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                sqlConnection.Close();
            }
        }


        // Delete a player by ID
        public int DeletePlayer(int playerId, out string errormsg)
        {
            string sqlString = "DELETE FROM tbl_player WHERE pl_id = @PlayerId;";

            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerId);

            try
            {
                sqlConnection.Open();
                int i = 0;
                i = sqlCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "Delete failed"; }
                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                sqlConnection.Close();
            }

        }

    }

}
