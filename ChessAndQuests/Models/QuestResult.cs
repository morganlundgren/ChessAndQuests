namespace ChessAndQuests.Models
{
    public class QuestResult
    {
        public PlayerQuestDetails PlayerQuest { get; set; }
        public bool QuestCompleted { get; set; }
        public QuestDetails QuestInfo { get; set; }
        public int? ExtraTurnPlayerId { get; set; }
        public QuestResult() { }
    }
}
