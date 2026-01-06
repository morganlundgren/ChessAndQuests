using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;

namespace ChessAndQuests.DAL
{
    public class PlayerQuestMethods
    {
        private SqlConnection sqlConnection;
        private string conString;
        public PlayerQuestMethods()
        {
            sqlConnection = new SqlConnection();
            conString = "Server=tcp:chessandquest-server.database.windows.net,1433;Initial Catalog=chessandquest-database;Persist Security Info=False;User ID=chessandquest-server-admin;Password=ilovechess123.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
            sqlConnection.ConnectionString = conString;

        } 

        // Create playerQuest
        public int CreatePlayerQuest(PlayerQuestDetails playerQuest, out string errormsg)
            {
            
            string sqlString = "INSERT INTO tbl_player_quest (gm_id, pl_id, qu_id, pq_currentmoves, pq_status, pq_progressmoves) VALUES (@GameId, @PlayerId, @QuestId, @CurrentMoves, @Status, @Progress)";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", playerQuest.GameId);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerQuest.PlayerId);
            sqlCommand.Parameters.AddWithValue("@QuestId", playerQuest.QuestId);
            sqlCommand.Parameters.AddWithValue("@CurrentMoves", playerQuest.PlayerQuestCurrentMove);
            sqlCommand.Parameters.AddWithValue("@Status", playerQuest.PlayerQuestStatus);
            sqlCommand.Parameters.AddWithValue("@Progress", playerQuest.ProgressMoves);
            
            try
            {
                sqlConnection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "No rows were deleted.";
                }
                return rowsAffected;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        // Update playerQuest status or current move
        public int UpdatePlayerQuest(PlayerQuestDetails playerQuest, out string errormsg)
        {
            string sqlString = "UPDATE tbl_player_quest SET pq_currentmoves = @CurrentMoves, pq_status = @Status, pq_progressmoves = @Progress WHERE pl_id = @PlayerId AND qu_id = @QuestId";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@CurrentMoves", playerQuest.PlayerQuestCurrentMove);
            sqlCommand.Parameters.AddWithValue("@Status", playerQuest.PlayerQuestStatus);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerQuest.PlayerId);
            sqlCommand.Parameters.AddWithValue("@QuestId", playerQuest.QuestId);
            sqlCommand.Parameters.AddWithValue("@Progress", playerQuest.ProgressMoves);
            try
            {
                sqlConnection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "No rows were updated.";
                }
                return rowsAffected;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }




        // Delete a quest after completion or max attempts
        public int DeletePlayerQuest(int playerId, int questId, out string errormsg)
        {
            string sqlString = "DELETE FROM tbl_player_quest WHERE pl_id = @PlayerId AND qu_id = @QuestId";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerId);
            sqlCommand.Parameters.AddWithValue("@QuestId", questId);
            try
            {
                sqlConnection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "No rows were deleted.";
                    
                }
                return rowsAffected;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
           
        }

        public PlayerQuestDetails GetPlayerQuestByGameandPlayer ( int gameId, int playerId, out string errormsg)
        {
            string sqlString = "SELECT * FROM tbl_player_quest WHERE gm_id = @GameId AND pl_id = @PlayerId" ;
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", gameId);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerId);

            SqlDataReader reader = null;
            PlayerQuestDetails playerQuest = new PlayerQuestDetails();
            try
            {
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (!reader.HasRows)
                {
                    errormsg = "No data found";
                    return null;
                }
                while (reader.Read())
                {
   
                    playerQuest.PlayerId = Convert.ToInt32(reader["pl_id"]);
                    playerQuest.QuestId = Convert.ToInt32(reader["qu_id"]);
                    playerQuest.GameId = Convert.ToInt32(reader["gm_id"]);
                    playerQuest.PlayerQuestCurrentMove = Convert.ToInt32(reader["pq_currentmoves"]);
                    playerQuest.PlayerQuestStatus = Convert.ToInt32(reader["pq_status"]);
                    playerQuest.ProgressMoves = Convert.ToInt32(reader["pq_progressmoves"]);

                }
                errormsg = "";
                return playerQuest;
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
        public int NextQuest( int gameId, int questId, out string errormsg)
        {
            

            string sqlString = "UPDATE tbl_player_quest SET qu_id = @QuestId, pq_currentmoves = @CurrentMoves, pq_status = @Status, pq_progressmoves = @Progress WHERE gm_id = @GameId";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", gameId);
            sqlCommand.Parameters.AddWithValue("@CurrentMoves", 0);
            sqlCommand.Parameters.AddWithValue("@Status", 0);
            sqlCommand.Parameters.AddWithValue("@QuestId", questId);
            sqlCommand.Parameters.AddWithValue("@Progress", 0);
            try
            {
                sqlConnection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "No rows were updated.";
                }
                return rowsAffected;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }


        }





    }
}
