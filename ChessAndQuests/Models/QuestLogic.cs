
using ChessAndQuests.Models;
using ChessAndQuests.DAL;

namespace ChessAndQuests.Models
{
    public class QuestLogic
    {
        private readonly PlayerQuestMethods playerQuestMethods;
        private readonly QuestMethods questMethods;

        public QuestLogic()
        {
            playerQuestMethods = new PlayerQuestMethods();
            questMethods = new QuestMethods();
        }

        public void HandleMove(PlayerQuestDetails playerQuest, GameViewModel gameViewModel)
        {
            if (playerQuest.PlayerQuestStatus == 1)
            {
                return;
            }

            switch (playerQuest.QuestId) 
            
            { 
                case 1:
                    if(gameViewModel.CapturedPiece)
                    
            }

        }
    }
}
