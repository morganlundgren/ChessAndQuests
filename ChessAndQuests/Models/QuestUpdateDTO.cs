namespace ChessAndQuests.Models
{
    public class QuestUpdateDTO
    {
        public PlayerQuestDetails WhitePlayerQuest { get; set; }
        public PlayerQuestDetails BlackPlayerQuest { get; set; }
        public bool QuestCompleted { get; set; }
        public QuestDetails CurrentQuest { get; set; }
        public QuestDetails? CompletedQuest { get; set; }
        public int? QuestWinnerId { get; set; }
    }
}
