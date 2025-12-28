
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

            bool questCompleted = false;

            switch (playerQuest.QuestId)

            {
                case 1: // pawn collector
                    if (gameViewModel.CapturedPiece == "p")
                    {
                        questCompleted = true;
                    }
                    break;

                case 2: // Knight March
                    if (gameViewModel.MovedPiece == "n")
                        playerQuest.PlayerQuestCurrentMove++;
                    else
                        playerQuest.PlayerQuestCurrentMove = 0;

                    if (playerQuest.PlayerQuestCurrentMove >= 3)
                        questCompleted = true;
                    break;

                case 3: // First Capture
                    if (!string.IsNullOrEmpty(gameViewModel.CapturedPiece))
                        questCompleted = true;
                    break;

                case 4: // Center Control
                    if (IsCenterSquare(gameViewModel.ToSquare))
                        questCompleted = true;
                    break;

                case 5: // Queen's Move
                    if (gameViewModel.MovedPiece == "q" && Distance(gameViewModel.FromSquare, gameViewModel.ToSquare) >= 2)
                        questCompleted = true;
                    break;

                case 6: // Rook Rampage
                    if (gameViewModel.MovedPiece == "r" && HorizontalDistance(gameViewModel.FromSquare, gameViewModel.ToSquare) >= 2)
                        questCompleted = true;
                    break;

                case 7: // Knight Pressure
                    if (gameViewModel.MovedPiece == "n" && ThreatensOpponentPiece(gameViewModel))
                        questCompleted = true;
                    break;

                case 8: // Capture Pawn (ta 3 bönder)
                    if (gameViewModel.CapturedPiece == "p")
                        playerQuest.PlayerQuestCurrentMove++;

                    if (playerQuest.PlayerQuestCurrentMove >= 3)
                        questCompleted = true;
                    break;

                case 9: // Table Has Turned
                    if (gameViewModel.MovedPiece == "k")
                        playerQuest.PlayerQuestCurrentMove++;
                    else
                        playerQuest.PlayerQuestCurrentMove = 0;

                    if (playerQuest.PlayerQuestCurrentMove >= 5)
                        questCompleted = true;
                    break;
                 }
            }

            private void CompleteQuest(PlayerQuestDetails pq)
        {
            pq.PlayerQuestStatus = 1; // markera quest som klar

            // Hämta questen från DB
            var quest = questMethods.GetQuestDetails(pq.QuestId, out string err);
            if (quest == null)
                return; // felhantering

            var reward = quest.QuestReward;

            // Belöning kan skickas till klienten via SignalR
            // t.ex. Clients.Caller.SendAsync("QuestReward", pq.PlayerId, pq.QuestId, reward);

            // Uppdatera quest-status i DB
            playerQuestMethods.UpdatePlayerQuest(pq, out _);
        }


    }

}


