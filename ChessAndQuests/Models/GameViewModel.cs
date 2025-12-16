namespace ChessAndQuests.Models
{
    public class GameViewModel
    {
        public string GameKey { get; set; }
        public string PlayerWhiteName { get; set; }
        public string PlayerBlackName { get; set; }
        public string CurrentFEN { get; set; }
        public string FromSquare { get; set; }
        public string ToSquare { get; set; }
        public string MoveNumber { get; set; }
        public int TurnPlayerId { get; set; }

        public GameViewModel() { }
    }
}
