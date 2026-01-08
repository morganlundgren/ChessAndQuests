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
        public string MovedPiece { get; set; }
        public string CapturedPiece { get; set; }

        public QuestDetails CurrentQuest { get; set; }

        public int PlayerQuestCurrentMove { get; set; }
        public int PlayerQuestStatus { get; set; }
        public int PlayerQuestProgressMoves { get; set; }
        public PlayerQuestDetails WhitePlayerQuest { get; set; }
        public PlayerQuestDetails BlackPlayerQuest { get; set; }
        public bool QuestCompleted { get; set; }
        public QuestDetails? CompletedQuest { get; set; }
        public int? QuestWinnerId { get; set; }
        public GameViewModel() { }
    }
}
