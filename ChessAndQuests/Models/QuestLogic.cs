
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

        public QuestResult HandleMove(PlayerQuestDetails playerQuest, GameViewModel gameViewModel)
        {
          
            var quest = questMethods.GetQuestDetails(playerQuest.QuestId, out string err);
            if (quest == null)
                return null;
            if (playerQuest.PlayerQuestStatus == 1)
            {
                return null;
            }

            bool questCompleted = false;

            if (playerQuest.ThreatHighlightActivated)
            {
                playerQuest.ThreatHighlightMovesLeft--;
                if (playerQuest.ThreatHighlightMovesLeft <= 0)
                {
                    playerQuest.ThreatHighlightActivated = false;
                    playerQuest.ThreatHighlightMovesLeft = 0;
                }
            }

            playerQuest.PlayerQuestCurrentMove++;
            if (playerQuest.PlayerQuestCurrentMove >= quest.QuestMaxMoves)
            {
                questCompleted = true;
            }
            else {
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
                            questCompleted = true; //borde vara quest nr1
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

                    case 8: // Capture Pawn (ta 3 bönder) //här är logiken lite tokig.
                        if (gameViewModel.CapturedPiece == "p")
                        {
                            playerQuest.ProgressMoves++;
                        }
                        if (playerQuest.ProgressMoves >= 3)
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
            }
            playerQuestMethods.UpdatePlayerQuest(playerQuest, out _);

            QuestResult questResult = new QuestResult
            {
                PlayerQuest = playerQuest,
                QuestCompleted = questCompleted,
                QuestInfo = quest
            }; ;

            if (questCompleted)
            {
                questResult = CompleteQuest(playerQuest);
            }
            return questResult;
        }
        //ska inte anropa singalR. Ska skicka tillbaka den uppdaterade playerquesten, och questets reward och sen anropa från controllern
        private QuestResult CompleteQuest(PlayerQuestDetails pq)
        {
            int? extraTurnPlayerId = null;
            pq.PlayerQuestStatus = 1; // markera quest som klar
            playerQuestMethods.UpdatePlayerQuest(pq, out _);
            var completedQuest = questMethods.GetQuestDetails(pq.QuestId, out _);
          

            switch (completedQuest.QuestRewards)
            {
                case "EXTRA_TURN":
                    extraTurnPlayerId = pq.PlayerId;
                    break;

                case "HIGHLIGHT_THREATS":
              
                    pq.ThreatHighlightActivated = true;
                    pq.ThreatHighlightMovesLeft = 5; 
                    break;

            }
            playerQuestMethods.UpdatePlayerQuest(pq, out _);

            // Uppdatera quest-status i DB
            int nextQuestId = pq.QuestId + 1;
            playerQuestMethods.NextQuest(pq.GameId, nextQuestId, out _);
            
            var nextQuest = questMethods.GetQuestDetails(nextQuestId, out _);
            var nextPLayerQuest = playerQuestMethods.GetPlayerQuestByGameandPlayer(pq.GameId, pq.PlayerId, out _);
            if (pq.PlayerQuestCurrentMove >= completedQuest.QuestMaxMoves)
            {
                return new QuestResult
                {
                    PlayerQuest = nextPLayerQuest,
                    QuestCompleted = true,
                    QuestInfo = nextQuest,
                    CompletedQuest = completedQuest,
                    ExtraTurnPlayerId = null,
                    QuestWinnerId = null
                };
            }
            return new QuestResult
            {
                PlayerQuest = nextPLayerQuest,
                QuestCompleted = true,
                QuestInfo = nextQuest,
                CompletedQuest = completedQuest,
                ExtraTurnPlayerId = extraTurnPlayerId,
                QuestWinnerId = pq.PlayerId

            };

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

        // Uppdatera FEN-strängen för att sätta rätt spelares tur, genom att skicka vems tur det ska vara
        public string SetTurn (string fen, int turnPlayerId, GameDetails game)
        {
            var fenParts = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries); //delar upp fensträngen i olika delar 

            if (fenParts.Length < 2)
            {
                return fen; 
            }
            if (turnPlayerId == game.PlayerWhiteId) // Ändrar fen strängen ifall en spelare får köra igen.
            {
                fenParts[1] = "w"; 
            }
            else
            {
                fenParts[1] = "b"; 
            }
            fen = string.Join(" ", fenParts);
            return fen;

        }
        /*public string GetLastMove()
        {

            return fenToRestore
        }*/
    }
}



