using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChessAndQuests.DAL
{
    public class QuestMethods
    {
        private SqlConnection sqlConnection;
        private string conString;
        public QuestMethods() {
            sqlConnection = new SqlConnection();
            conString = "Server=tcp:chessandquest-server.database.windows.net,1433;Initial Catalog=chessandquest-database;Persist Security Info=False;User ID=chessandquest-server-admin;Password=ilovechess123.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
            sqlConnection.ConnectionString = conString;
        }

        public List<QuestDetails> GetAllQuests(out string errormsg)
        {
            string sqlString = "SELECT * FROM tbl_quest";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);

            SqlDataReader reader = null;
            List<QuestDetails> questDetailsList = new List<QuestDetails>();
            try
            {
                // Fyll dataset och mappa rader till modeller
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    QuestDetails questDetails = new QuestDetails();

                    questDetails.QuestID = Convert.ToUInt16(reader["qu_Id"]);
                    questDetails.QuestName = Convert.ToString(reader["qu_name"]);
                    questDetails.QuestDescription = Convert.ToString(reader["qu_description"]);
                    questDetails.QuestMaxMoves = Convert.ToUInt16(reader["qu_max_moves"]);
                    questDetails.QuestRewards = Convert.ToString(reader["qu_reward"]);
                    questDetails.QuestMaxProgressMoves = (reader["qu_max_progress_moves"]) != DBNull.Value ? Convert.ToUInt16(reader["qu_max_progress_moves"]) : null;
                    questDetailsList.Add(questDetails);
                }
                errormsg = "";
                return questDetailsList;
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
                {
                    reader.Close();
                }
                    sqlConnection.Close();
            }
        }
        public QuestDetails GetQuestDetails(int id, out string errormsg) {
            string sqlString = "SELECT * FROM tbl_quest WHERE qu_id = @Id";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Id", id);
            SqlDataReader reader = null;
            QuestDetails questDetails = new QuestDetails();
            try
            {
                // Fyll dataset och mappa rader till modeller
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    questDetails.QuestID = Convert.ToUInt16(reader["qu_Id"]);
                    questDetails.QuestName = Convert.ToString(reader["qu_name"]);
                    questDetails.QuestDescription = Convert.ToString(reader["qu_description"]);
                    questDetails.QuestMaxMoves = Convert.ToUInt16(reader["qu_max_moves"]);
                    questDetails.QuestRewards = Convert.ToString(reader["qu_reward"]);
                    questDetails.QuestMaxProgressMoves = (reader["qu_max_progress_moves"]) != DBNull.Value ? Convert.ToUInt16(reader["qu_max_progress_moves"]) : null;

                }
                errormsg = "";
                return questDetails;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                    sqlConnection.Close();
            }
        }
    }
}
