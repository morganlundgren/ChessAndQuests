using System.Data;
using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

// Data Access Layer for Move-related database operations
namespace ChessAndQuests.DAL
{
    public class MoveMethods
    {
        private SqlConnection sqlConnection;
        private string conString;
        public MoveMethods()
        {
            sqlConnection = new SqlConnection();
            conString = "Server=tcp:chesserver.database.windows.net,1433;Initial Catalog=chessquestserver;Persist Security Info=False;User ID=adminlogin;Password=ilovechess123.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            sqlConnection.ConnectionString = conString;
        }


        //
        public List<MoveDetails> GetMoves(int gameId, out string errormsg)
        {
            String sqlString = "SELECT * FROM tbl_move WHERE gm_id = @GameId";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", gameId);
            SqlDataReader reader = null;
            List<MoveDetails> moveDetailsList = new List<MoveDetails>();
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
                    MoveDetails moveDetails = new MoveDetails();
                    moveDetails.MoveId = Convert.ToUInt16(reader["mv_Id"]);
                    moveDetails.GameId = Convert.ToUInt16(reader["gm_id"]);
                    moveDetails.PlayerMoveId = Convert.ToUInt16(reader["pl_move_id"]);
                    moveDetails.MoveNumber = Convert.ToUInt16(reader["mv_number"]);
                    moveDetails.FromSquare = reader["mv_fr_square"].ToString() ?? "";
                    moveDetails.ToSquare = reader["mv_to_square"].ToString() ?? "";
                    moveDetailsList.Add(moveDetails);
                }
                errormsg = "";
                return moveDetailsList;
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
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }


        
        public int create(MoveDetails moveDetails, out string errormsg)
        {
            String sqlString = "INSERT INTO tbl_move (gm_id, pl_move_id, mv_number, mv_fr_square, mv_to_square) " +
                "OUTPUT INSERTED.mv_Id VALUES (@GameId, @PlayerMoveId, @MoveNumber, @MoveFromSquare, @MoveToSquare)";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", moveDetails.GameId);
            sqlCommand.Parameters.AddWithValue("@PlayerMoveId", moveDetails.PlayerMoveId);
            sqlCommand.Parameters.AddWithValue("@MoveNumber", moveDetails.MoveNumber);
            sqlCommand.Parameters.AddWithValue("@MoveFromSquare", moveDetails.FromSquare);
            sqlCommand.Parameters.AddWithValue("@MoveToSquare", moveDetails.ToSquare);
            try
            {
                sqlConnection.Open();
                int insertedId = (int)sqlCommand.ExecuteScalar();
                errormsg = "";
                return insertedId;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }

        }
        // Update existing move
        public int Update(MoveDetails moveDetails, out string errormsg)
        {
            String sqlString = "UPDATE tbl_move SET gm_id = @GameId, pl_move_id = @PlayerMoveId, mv_number = @MoveNumber, " +
                "mv_fr_square = @MoveFromSquare, mv_to_square = @MoveToSquare WHERE mv_id = @MoveId";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", moveDetails.GameId);
            sqlCommand.Parameters.AddWithValue("@PlayerMoveId", moveDetails.PlayerMoveId);
            sqlCommand.Parameters.AddWithValue("@MoveNumber", moveDetails.MoveNumber);
            sqlCommand.Parameters.AddWithValue("@MoveFromSquare", moveDetails.FromSquare);
            sqlCommand.Parameters.AddWithValue("@MoveToSquare", moveDetails.ToSquare);
            sqlCommand.Parameters.AddWithValue("@MoveId", moveDetails.MoveId);
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

    }
}
