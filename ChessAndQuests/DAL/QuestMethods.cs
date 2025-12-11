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
            conString = "Server=tcp:chesserver.database.windows.net,1433;Initial Catalog=chessquestserver;Persist Security Info=False;User ID=adminlogin;Password=ilovechess123.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            sqlConnection.ConnectionString = conString;
        }

        public List<QuestDetails> GetAllQuests(out string errormsg)
        {
            string sqlString = "SELECT * FROM tbl_quests";
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
                    questDetails.QuestID = Convert.ToUInt16(reader["qt_Id"]);
                    questDetails.QuestName = Convert.ToString(reader["qt_name"]);
                    questDetails.QuestDescription = Convert.ToString(reader["qt_description"]);
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
    }
}
