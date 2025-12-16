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
            //hämta fensträng för startposition
            GameDetails newGame = new GameDetails();
            {
                newGame.PLayerWhiteId = playerId;
                newGame.GameKey = gamekey;
                newGame.CurrentFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\r\n";
                newGame.status = 0;
                newGame.turnId = playerId;
            }
            int i = 0;
            string error = "";

            i = gameMethods.CreateGame(newGame, out error);

            if (i == 0)
            {
                ViewBag.ErrorGame = "Error creating game: " + error;
                return View();
            }
            PlayerMethods playerMethods = new PlayerMethods();
            var player = playerMethods.GetById(playerId, out error);

            return RedirectToAction("PlayGame","Game", new { gameKey = newGame.GameKey });
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
            string error = "";
            int i = 0;
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
            i = gameMethods.UpdateGame(gameToJoin, out error);
            if(i==0)
            {
                ViewBag.ErrorJoin = "Error joining game: " + error;
                return View();
            }


            return RedirectToAction("PlayGame", "Game", new { gameKey = gameToJoin.GameKey });
        }

        [HttpGet]
        public IActionResult PlayGame(string gameKey)
        {
            if (HttpContext.Session.GetString("PlayerUsername") == null)
            {
                return RedirectToAction("SignIn", "Player");
            }
            GameMethods gameMethods = new GameMethods();
            GameDetails gameDetails = new GameDetails();
            PlayerDetails playerBlack = null;
            string error = "";

            gameDetails= gameMethods.GetGameByKey(gameKey, out  error);

            var model = new GameViewModel
            {
                GameKey = gameKey,
                CurrentFEN = gameDetails.CurrentFEN

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MakeMove(GameViewModel gamevm)
        {
            //get game by key
            var moveMethods = new MoveMethods();
            var gameMethods = new GameMethods();
            var game = gameMethods.GetGameByKey(gamevm.GameKey, out var error);
            if (game == null) {return BadRequest("Game not found: " + error);}

            //update game's current fen
            game.CurrentFEN = gamevm.CurrentFEN?.Trim() ?? game.CurrentFEN;
            
            gameMethods.UpdateGame(game, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest("Error updating game: " + error);
            }

            var previousMoves = moveMethods.GetMoves(game.GameId, out error); // get previous moves by player HERE!!!
            int moveNumber = previousMoves.Count + 1;

            var moveDetails = new MoveDetails
            {
                GameId = game.GameId,
                FromSquare = gamevm.FromSquare,
                ToSquare = gamevm.ToSquare,
                PlayerMoveId = (gamevm.TurnPlayerId == game.PLayerWhiteId) ? game.PlayerBlackId.Value : game.PLayerWhiteId,
                MoveNumber = moveNumber
            };

            moveMethods.create(moveDetails, out error);



            // update moves

            //notify clients in the game group about the move
            await _gameHubContext.Clients.Group(gamevm.GameKey).SendAsync("ReceiveLatestFen", game.CurrentFEN);

            return Ok();
        }

    }
}

