
using ChessAndQuests.DAL;
using ChessAndQuests.Hubs;
using ChessAndQuests.Models;
using ChessDotNet;
using Microsoft.AspNetCore.SignalR;

namespace ChessAndQuests.Models
{
    public class QuestLogic
    {
        private readonly PlayerQuestMethods playerQuestMethods;
        private readonly QuestMethods questMethods;
        private readonly GameMethods gameMethods;
        private readonly IHubContext<GameHub> _hubContext;

        public QuestLogic (IHubContext<GameHub> GameHubContext)
        {

            _hubContext = GameHubContext;
            playerQuestMethods = new PlayerQuestMethods();
            questMethods = new QuestMethods();
            gameMethods = new GameMethods();

        }

        //borde vi uppdatera playerquest i databasen också? hur ska det annars visas i vyn? 
        //(t.ex currentmoves, progressmoves osv)

        public PlayerQuestDetails HandleMove(PlayerQuestDetails playerQuest, GameViewModel gameViewModel)
        {
            if (playerQuest.PlayerQuestStatus == 1)
            {
                return playerQuest;
            }

            bool questCompleted = false;
            playerQuest.PlayerQuestCurrentMove++;
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
                        playerQuest.ProgressMoves++;
                    else
                        playerQuest.ProgressMoves = 0;

                    if (playerQuest.ProgressMoves >= 3)
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

                case 8: // Capture Pawn (ta 3 bönder) //??
                    if (gameViewModel.CapturedPiece == "p")
                    if (playerQuest.PlayerQuestCurrentMove >= 3)
                        questCompleted = true;
                    break;

                case 9: // Table Has Turned
                    if (gameViewModel.MovedPiece == "k")
                        playerQuest.ProgressMoves++;
                    else
                        playerQuest.ProgressMoves = 0;

                    if (playerQuest.ProgressMoves >= 5)
                        questCompleted = true;
                    break;
                 }
            playerQuestMethods.UpdatePlayerQuest(playerQuest, out _);


            if (questCompleted)
            {
                CompleteQuest(playerQuest, gameViewModel.GameKey);
            }
            return playerQuest;

        }
        //ska inte anropa singalR. Ska skicka tillbaka den uppdaterade playerquesten, och questets reward och sen anropa från controllern
        private void CompleteQuest(PlayerQuestDetails pq, string gameKey)
        {
            pq.PlayerQuestStatus = 1; // markera quest som klar
            playerQuestMethods.UpdatePlayerQuest(pq, out _);


            // Hämta questen från DB
            var quest = questMethods.GetQuestDetails(pq.QuestId, out string err);
            if (quest == null)
                return;

            var reward = quest.QuestRewards;

            //------------------------clientID är inte playerID som string***------------------------
            _hubContext.Clients.Client(pq.PlayerId.ToString()).SendAsync("ReceiveQuestReward", new { pq.QuestId, quest.QuestRewards });
            //varför inte skicka till gruppen? Båda måste ju uppdateras. kan man kanske skicka med ett winnerPlayerId?

            _hubContext.Clients.Group(gameKey).SendAsync("QuestStatusUpdated", new {pq.QuestId, pq.PlayerId }); //vad ska denna göra?

            // Uppdatera quest-status i DB
            int nextQuestId = pq.QuestId + 1;
            playerQuestMethods.NextQuest(pq.GameId, nextQuestId, out _);

        }

        // Hjälpmetoder

        private bool IsCenterSquare(string square)
        {
            return square == "d4" || square == "d5" || square == "e4" || square == "e5";
        }

        private int Distance(string from, string to)
        {
            int colDiff = Math.Abs(from[0] - to[0]);
            int rowDiff = Math.Abs(from[1] - to[1]);
            return colDiff + rowDiff;
        }

        private int HorizontalDistance(string from, string to)
        {
            return Math.Abs(from[0] - to[0]);
        }

        private bool ThreatensOpponentPiece(GameViewModel gamevm)
        {
            return true;
        }
    }
}



