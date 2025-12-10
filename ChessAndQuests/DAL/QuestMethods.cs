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
            conString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";
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
