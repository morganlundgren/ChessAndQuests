namespace ChessAndQuests.Models
{
    public class PlayerQuestDetails
    {
        public int PlayerQuestId { get; set; }
        public int GamaeId { get; set; }
        public int PlayerId { get; set; }
        public int QuestID { get; set; }
        public int PlayerQuestStatus { get; set; }
        public int PlayerQuestCurrentMove { get; set; }
    }
}
