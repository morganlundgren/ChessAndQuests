namespace ChessAndQuests.Models
{
    public class GameViewModel
    {
        public string GameKey { get; set; }
        public string PlayerWhiteName { get; set; }
        public int PlayerWhiteId { get; set; }
        public string PlayerBlackName { get; set; }
        public int? PlayerBlackId { get; set; }
        public string CurrentFEN { get; set; }
        public string FromSquare { get; set; }
        public string ToSquare { get; set; }
        public string MoveNumber { get; set; }
        public int TurnPlayerId { get; set; }
        public int MovedPiece { get; set; }
        public int CapturedPiece { get; set; }

        public QuestDetails Quest { get; set; }
        public int PlayerQuestStatus { get; set; }
        public int PlayerQuestCurrentMove { get; set; }
        public int ProgressMoves { get; set; }

        public GameViewModel() { }
    }
}
