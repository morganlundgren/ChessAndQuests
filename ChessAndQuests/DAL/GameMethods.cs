using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChessAndQuests.DAL
{
    


    public class GameMethods
    {
        private SqlConnection sqlConnection;
        private string conString;
        public GameMethods()

        {
            sqlConnection = new SqlConnection();
            conString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";
            sqlConnection.ConnectionString = conString;
        }
        public List<GameDetails> GetAllGames(out string errormsg)
        {
            String sqlString ="SELECT * FROM tbl_game";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);

            SqlDataReader reader = null;
            List<GameDetails> gameDetailsList = new List<GameDetails>();

            try
            {
                // Fyll dataset och mappa rader till modeller
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
               
                if (!reader.HasRows)
                {
                    errormsg = "No data found";
                    return null;
                }
                while (reader.Read())
                {
                    GameDetails gameDetails = new GameDetails();
                    gameDetails.GameId = Convert.ToUInt16(reader["gm_Id"]);
                    gameDetails.PLayerWhiteId = Convert.ToUInt16(reader["pl_white_id"]);
                    gameDetails.PlyerBlackId = Convert.ToUInt16(reader["pl_black_id"]);
                    gameDetails.GameKey = Convert.ToString(reader["gm_key"]);
                    gameDetails.CurrentFEN = Convert.ToString(reader["gm_current_fen"]);
                    gameDetails.status = Convert.ToUInt16(reader["gm_status"]);
                    gameDetails.turnId = Convert.ToUInt16(reader["gm_turn"]);

                    gameDetailsList.Add(gameDetails);
                }
                errormsg = "";
                return gameDetailsList;
              
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

                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }

    }
}
