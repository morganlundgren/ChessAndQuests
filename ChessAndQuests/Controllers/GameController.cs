using Microsoft.AspNetCore.Mvc;
using ChessAndQuests.Models;
using ChessAndQuests.DAL;

namespace ChessAndQuests.Controllers
{
    public class GameController : Controller
    {
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
            PlayerMethods playerMethods = new PlayerMethods();
            var playerWhite = playerMethods.GetById(gameDetails.PLayerWhiteId, out error);
            if (gameDetails.PlayerBlackId != null)
            {
                int playerBlackId = gameDetails.PlayerBlackId.GetValueOrDefault();
                playerBlack = playerMethods.GetById(playerBlackId, out error);
            }

            var model = new GameViewModel
            {
                GameKey = gameKey,
                PlayerWhiteName = playerWhite?.PlayerUserName ?? "Waiting...",
                PlayerBlackName = playerBlack?.PlayerUserName ?? "Waiting...",
                CurrentFEN = gameDetails.CurrentFEN

            };

            return View(model);
        }

    }
}

