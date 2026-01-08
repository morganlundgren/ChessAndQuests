namespace ChessAndQuests.Models
{
    public class QuestDetails
    {
        public int QuestID { get; set; }
        public string QuestName { get; set; }
        public string QuestDescription { get; set; }
        public int QuestMaxMoves { get; set; }

        public string QuestRewards { get; set; }  
        public int? QuestMaxProgressMoves { get; set; }
        public QuestDetails() { }

    }
}
