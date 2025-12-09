using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;

namespace ChessAndQuests.DAL
{
    


    public class GameMethods
    {
        private SqlConnection sqlconnection;
        private string conString;
        public GameMethods()

        {
            sqlconnection = new SqlConnection();
            conString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";
            sqlconnection.ConnectionString = conString;
        }
        public List<GameDetails> GetAllGames()
        {
            String sqlstring ="SELECT * FROM tbl_game";
        }
    }
}
