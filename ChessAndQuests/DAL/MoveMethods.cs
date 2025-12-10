using System.Data;
using ChessAndQuests.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace ChessAndQuests.DAL
{
    public class MoveMethods
    {
        private SqlConnection sqlConnection;
        private string conString;
        public MoveMethods()
        {
            sqlConnection = new SqlConnection();
            conString = "Data Source = chesserver.database.windows.net; User ID = adminlogin; Password = ********; Connect Timeout = 30; Encrypt = True; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False";
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
                    moveDetails.PlayerMoveId = Convert.ToUInt16(reader["pl_id"]);
                    moveDetails.MoveNumber = Convert.ToUInt16(reader["mv_number"]);
                    moveDetails.MoveFromSquare = reader["mv_from_square"].ToString() ?? "";
                    moveDetails.MoveToSquare = reader["mv_to_square"].ToString() ?? "";
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
            String sqlString = "INSERT INTO tbl_move (gm_id, pl_id, mv_number, mv_from_square, mv_to_square) OUTPUT INSERTED.mv_Id VALUES (@GameId, @PlayerMoveId, @MoveNumber, @MoveFromSquare, @MoveToSquare)";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", moveDetails.GameId);
            sqlCommand.Parameters.AddWithValue("@PlayerMoveId", moveDetails.PlayerMoveId);
            sqlCommand.Parameters.AddWithValue("@MoveNumber", moveDetails.MoveNumber);
            sqlCommand.Parameters.AddWithValue("@MoveFromSquare", moveDetails.MoveFromSquare);
            sqlCommand.Parameters.AddWithValue("@MoveToSquare", moveDetails.MoveToSquare);
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
                return -1;
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
            String sqlString = "UPDATE tbl_move SET gm_id = @GameId, pl_id = @PlayerMoveId, mv_number = @MoveNumber, mv_from_square = @MoveFromSquare, mv_to_square = @MoveToSquare WHERE mv_id = @MoveId";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@GameId", moveDetails.GameId);
            sqlCommand.Parameters.AddWithValue("@PlayerMoveId", moveDetails.PlayerMoveId);
            sqlCommand.Parameters.AddWithValue("@MoveNumber", moveDetails.MoveNumber);
            sqlCommand.Parameters.AddWithValue("@MoveFromSquare", moveDetails.MoveFromSquare);
            sqlCommand.Parameters.AddWithValue("@MoveToSquare", moveDetails.MoveToSquare);
            sqlCommand.Parameters.AddWithValue("@MoveId", moveDetails.MoveId);
            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                errormsg = "";
            }
            catch (Exception e)
            {
                errormsg = e.Message;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
            return moveDetails.MoveId;
        }

    }
}
