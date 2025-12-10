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
            conString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";
            sqlConnection.ConnectionString = conString;

        }

        // Create and assign a quest to a player
        public void AssignQuestToPlayer(int playerId, int questId, out string errormsg)
        {
            string sqlString = "INSERT INTO tbl_player_quests (pl_id, qt_id, pq_status) VALUES (@PlayerId, @QuestId, @Status)";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@PlayerId", playerId);
            sqlCommand.Parameters.AddWithValue("@QuestId", questId);
            sqlCommand.Parameters.AddWithValue("@Status", 0); // Assuming 0 is the default status for a new quest assignment
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
                    errormsg = "No rows were inserted.";
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
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
