using ChessAndQuests.DAL;
using ChessAndQuests.Hubs;
using ChessAndQuests.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ChessAndQuests.Controllers
{
    public class GameController : Controller
    {
        private readonly IHubContext<GameHub> _gameHubContext;
        public GameController(IHubContext<GameHub> gameHubContext)
        {
            _gameHubContext = gameHubContext;
        }

        public IActionResult TestBoard()
        {
            return View();
        }

        [HttpGet]
        // create a new game get
        public IActionResult CreateGame()
        {
            if (HttpContext.Session.GetString("PlayerUsername") == null)
            {
                return RedirectToAction("SignIn", "Player");
            }
            return View();
        }

        [HttpPost]
        // create a new game post
        //en funktion som tar in indata och skapar ett nytt spel och returnerar ett GameDetails objekt
        //Skapa nytt spel i dal GameMethods.CreateGame(GameDetails)
        //returnera till spelbrädssidan med det nya spelet
        public IActionResult CreateGame(string gamekey, int playerId)
        {
            GameMethods gameMethods = new GameMethods();
            QuestMethods questMethods = new QuestMethods();
            PlayerQuestMethods playerQuestMethods = new PlayerQuestMethods();
            //hämta fensträng för startposition
            GameDetails newGame = new GameDetails();
            {
                newGame.PlayerWhiteId = playerId;
                newGame.GameKey = gamekey;
                newGame.CurrentFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\r\n";
                newGame.status = 0;
                newGame.turnId = playerId;
            }

            int iGame = 0;
            string error = "";
            int ipq = 0;
          

            if (gameMethods.GetGameByKey(gamekey, out error) != null)
            {
                ViewBag.ErrorGame = "Game key already exists. Please choose another one.";
                return View();
            }
            
            iGame = gameMethods.CreateGame(newGame, out error);

            if (iGame == 0)
            {
                ViewBag.ErrorGame = "Error creating game: " + error;
                return View();
            }
            newGame = gameMethods.GetGameByKey(newGame.GameKey, out error);
            PlayerMethods playerMethods = new PlayerMethods();
            var player = playerMethods.GetById(playerId, out error);

            var firstQuest = questMethods.GetAllQuests(out error)?.FirstOrDefault();
            var firstPlayerQuest = new PlayerQuestDetails
            {
                PlayerId = playerId,
                QuestId = firstQuest.QuestID,
                GameId = newGame.GameId,
                PlayerQuestStatus = 0,
                PlayerQuestCurrentMove = 0,
                ProgressMoves = 0,
                ThreatHighlightActivated = false,
                ThreatHighlightMovesLeft = 0
            };
            ipq = playerQuestMethods.CreatePlayerQuest(firstPlayerQuest, out error);
            if (ipq == 0)
            {
                ViewBag.ErrorGame = error;
                return View();
            }

            return RedirectToAction("PlayGame", "Game", new { gameKey = newGame.GameKey, quest = firstQuest, playerQuest = firstPlayerQuest });
        }


        [HttpGet]
        // join a game get
        public IActionResult JoinGame()
        {
            if (HttpContext.Session.GetString("PlayerUsername") == null)
            {
                return RedirectToAction("SignIn", "Player");
            }


            return View();

        }


        [HttpPost]
        // join a game post
        public IActionResult JoinGame(string gamekey, int playerId)
        {

            GameMethods gameMethods = new GameMethods();
            QuestMethods questMethods = new QuestMethods();
            PlayerQuestMethods playerQuestMethods = new PlayerQuestMethods();
            string error = "";
            int ipq = 0;
            int iGame = 0;
            GameDetails gameToJoin = gameMethods.GetGameByKey(gamekey, out error);

            if (gameToJoin == null)
            {
                ViewBag.ErrorJoin = "Game not found.";
                return View();
            }
            if (gameToJoin.PlayerBlackId != null)
            {
                ViewBag.ErrorJoin = "Game is already full.";
                return View();
            }
            PlayerMethods playerMethods = new PlayerMethods();
            var player = playerMethods.GetById(playerId, out error);
            gameToJoin.PlayerBlackId = playerId;
            iGame = gameMethods.UpdateGame(gameToJoin, out error);
            if (iGame == 0)
            {
                ViewBag.ErrorJoin = "Error joining game: " + error;
                return View();
            }

            var firstQuest = questMethods.GetAllQuests(out error)?.FirstOrDefault();
            var firstPlayerQuest = new PlayerQuestDetails
            {
                PlayerId = playerId,
                QuestId = firstQuest.QuestID,
                GameId = gameToJoin.GameId,
                PlayerQuestStatus = 0,
                PlayerQuestCurrentMove = 0,
                ProgressMoves = 0,
                ThreatHighlightActivated = false,
                ThreatHighlightMovesLeft = 0
            };
            ipq = playerQuestMethods.CreatePlayerQuest(firstPlayerQuest, out error);
            if (ipq == 0)
            {
                ViewBag.playerQuest = error;
                return View();
            }

            return RedirectToAction("PlayGame", "Game", new { gameKey = gameToJoin.GameKey, quest = firstQuest, playerQuest = firstPlayerQuest });
        }

        [HttpGet]
        public IActionResult PlayGame(string gameKey, QuestDetails quest, PlayerQuestDetails playerQuest)
        {
            if (HttpContext.Session.GetString("PlayerUsername") == null)
            {
                return RedirectToAction("SignIn", "Player");
            }
            GameMethods gameMethods = new GameMethods();
            GameDetails gameDetails = new GameDetails();
            string error = "";

            gameDetails = gameMethods.GetGameByKey(gameKey, out error);



            var model = new GameViewModel
            {
                GameKey = gameKey,
                CurrentFEN = gameDetails.CurrentFEN,
                CurrentQuest = quest,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MakeMove([FromBody] GameViewModel gamevm)
        {
            //get game by key
            var moveMethods = new MoveMethods();
            var gameMethods = new GameMethods();
            var questMethods = new QuestMethods();
            var playerquestsMethods = new PlayerQuestMethods();
            var questLogic = new QuestLogic(_gameHubContext);

            string error = "";
            int iGame = 0;
            int iMove = 0;
            var game = gameMethods.GetGameByKey(gamevm.GameKey, out error);
            int moveNumber =1;
            if (game == null) {ViewBag.Game("Game not found: " + error);}

            var previousMoves = moveMethods.GetMoves(game.GameId, out error); // get previous moves by player HERE!!!

            moveNumber = previousMoves != null ? previousMoves.Count + 1 : moveNumber;
            //ska spara alla fen strängar;
            var move = new MoveDetails
            {
                GameId = game.GameId,
                FromSquare = gamevm.FromSquare,
                ToSquare = gamevm.ToSquare,
                PlayerMoveId = gamevm.TurnPlayerId,
                MoveNumber = moveNumber
            };

            iMove = moveMethods.create(move, out error);
            if (iMove == 0)
            {
                ViewBag.errorMove = error;
            }

            

            //quest logic handling

            var activePlayerQuest = playerquestsMethods.GetPlayerQuestByGameandPlayer(game.GameId, gamevm.TurnPlayerId, out error);
            var questResult = questLogic.HandleMove(activePlayerQuest, gamevm); 
                                                                             
            bool extraTurnGranted = false;

            //update game's current fen
            game.CurrentFEN = gamevm.CurrentFEN?.Trim() ?? game.CurrentFEN;
            if (questResult.ExtraTurnPlayerId.HasValue)
            {
                game.turnId = questResult.ExtraTurnPlayerId.Value; // behåll samma spelare
                extraTurnGranted = true;
            }
            else
            {       
                game.turnId = (gamevm.TurnPlayerId == game.PlayerWhiteId) ? game.PlayerBlackId.Value : game.PlayerWhiteId; 
            }
            game.CurrentFEN = questLogic.SetTurn(game.CurrentFEN, game.turnId, game);

            iGame = gameMethods.UpdateGame(game, out error);
            if (iGame == 0)
            {
               ViewBag.updateGame("Error updating game: " + error);
            }

            var whitePlayerQuest = playerquestsMethods.GetPlayerQuestByGameandPlayer(game.GameId, game.PlayerWhiteId, out error);
            var blackPlayerQuest = playerquestsMethods.GetPlayerQuestByGameandPlayer(game.GameId, game.PlayerBlackId.Value, out error);
            
            //samma sak för UNDO

            //notify clients in the game group about the move
            await _gameHubContext.Clients.Group(gamevm.GameKey).SendAsync("ReceiveLatestFen", new MoveInfoDTO
            {
                FromSquare = move.FromSquare,
                ToSquare = move.ToSquare,
                CurrentFEN = game.CurrentFEN,
                TurnPlayerId = game.turnId,
                ExtraTurnGranted = extraTurnGranted
            });
            await _gameHubContext.Clients.Group(gamevm.GameKey).SendAsync("UpdateQuest", new QuestUpdateDTO
            {
                WhitePlayerQuest = whitePlayerQuest,
                BlackPlayerQuest = blackPlayerQuest,
                QuestCompleted = questResult?.QuestCompleted ?? false,
                CurrentQuest = questResult.QuestInfo,
                CompletedQuest = questResult.CompletedQuest,
                QuestWinnerId = questResult?.QuestWinnerId ?? null,
            });

            return Ok();
        }

        // delete a game

        [HttpPost]
        public IActionResult DeleteGame([FromBody] GameViewModel gamevm)
        {
            GameMethods gameMethods = new GameMethods();
            string error = "";
            int i = 0;
            GameDetails gameToDelete = gameMethods.GetGameByKey(gamevm.GameKey, out error);
            if (gameToDelete == null)
            {
                ViewBag.ErrorDelete = "Game not found.";
            }
            i = gameMethods.DeleteGame(gameToDelete.GameId, out error);
            if (i == 0)
            {
                ViewBag.ErrorDelete = "Error deleting game: " + error;
            }
            return Ok();

        }
    }
}

