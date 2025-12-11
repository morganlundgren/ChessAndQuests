using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChessAndQuests.DAL
{
    // Data Access Layer för Game
    public class GameMethods
    {
        private SqlConnection sqlConnection;
        private string conString;
        public GameMethods()

        {
            sqlConnection = new SqlConnection();
            conString = "Server=tcp:chesserver.database.windows.net,1433;Initial Catalog=chessquestserver;Persist Security Info=False;User ID=adminlogin;Password=ilovechess123.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            sqlConnection.ConnectionString = conString;
        }
        public List<GameDetails> GetAllGames(out string errormsg)
        {
            String sqlString = "SELECT * FROM tbl_game";
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
                    gameDetails.PlayerBlackId = Convert.ToUInt16(reader["pl_black_id"]);
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

        public int CreateGame(GameDetails gameDetails, out string errormsg)
        {
            string sqlString = "INSERT INTO tbl_game (pl_white_id, pl_black_id, gm_key, gm_current_fen, gm_status, gm_turn) " +
                "VALUES (@pl_white_id, @pl_black_id, @gm_key, @gm_current_fen, @gm_status, @gm_turn); ";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@pl_white_id", gameDetails.PLayerWhiteId);
            sqlCommand.Parameters.AddWithValue("@pl_black_id", gameDetails.PlayerBlackId);
            sqlCommand.Parameters.AddWithValue("@gm_key", gameDetails.GameKey);
            sqlCommand.Parameters.AddWithValue("@gm_current_fen", gameDetails.CurrentFEN);
            sqlCommand.Parameters.AddWithValue("@gm_status", gameDetails.status);
            sqlCommand.Parameters.AddWithValue("@gm_turn", gameDetails.turnId);

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

        public int UpdateGame(GameDetails gameDetails, out string errormsg)
        {
            string sqlString = "UPDATE tbl_game SET pl_white_id = @pl_white_id, pl_black_id = @pl_black_id, gm_key = @gm_key, " +
                "gm_current_fen = @gm_current_fen, gm_status = @gm_status, gm_turn = @gm_turn WHERE gm_id = @gm_id;";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@pl_white_id", gameDetails.PLayerWhiteId);
            sqlCommand.Parameters.AddWithValue("@pl_black_id", gameDetails.PlayerBlackId);
            sqlCommand.Parameters.AddWithValue("@gm_key", gameDetails.GameKey);
            sqlCommand.Parameters.AddWithValue("@gm_current_fen", gameDetails.CurrentFEN);
            sqlCommand.Parameters.AddWithValue("@gm_status", gameDetails.status);
            sqlCommand.Parameters.AddWithValue("@gm_turn", gameDetails.turnId);
            sqlCommand.Parameters.AddWithValue("@gm_id", gameDetails.GameId);
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

        public int DeleteGame(int gameId, out string errormsg)
        {
            string sqlString = "DELETE FROM tbl_game WHERE gm_id = @gm_id;";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@gm_id", gameId);
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
