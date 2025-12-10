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

        //branch test
    }
}
