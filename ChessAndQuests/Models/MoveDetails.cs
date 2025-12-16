namespace ChessAndQuests.Models
{
    public class MoveDetails
    {
        public int MoveId { get; set; }
        public int GameId { get; set; }
        public int PlayerMoveId { get; set; }
        public int MoveNumber { get; set; }
        public string FromSquare { get; set; }
        public string ToSquare { get; set; }

    }
}
