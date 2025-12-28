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
            
            string sqlString = "INSERT INTO tbl_player_quests (gm_id, pl_id, qu_id, pq_currentmoves, pq_status) VALUES (@GameId, @PlayerId, @QuestId, @CurrentMoves, @Status)";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", playerQuest.GameId);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerQuest.PlayerId);
            sqlCommand.Parameters.AddWithValue("@QuestId", playerQuest.QuestId);
            sqlCommand.Parameters.AddWithValue("@CurrentMoves", playerQuest.PlayerQuestCurrentMove);
            sqlCommand.Parameters.AddWithValue("@Status", playerQuest.PlayerQuestStatus);
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
            string sqlString = "UPDATE tbl_player_quests SET pq_currentmoves = @CurrentMoves, pq_status = @Status WHERE pl_id = @PlayerId AND qu_id = @QuestId";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@CurrentMoves", playerQuest.PlayerQuestCurrentMove);
            sqlCommand.Parameters.AddWithValue("@Status", playerQuest.PlayerQuestStatus);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerQuest.PlayerId);
            sqlCommand.Parameters.AddWithValue("@QuestId", playerQuest.QuestId);
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
            string sqlString = "DELETE FROM tbl_player_quests WHERE pl_id = @PlayerId AND qu_id = @QuestId";
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





    }
}
