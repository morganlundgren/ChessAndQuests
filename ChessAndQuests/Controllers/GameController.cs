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

            return RedirectToAction("PlayGame", newGame.GameKey);
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
            GameDetails gameToJoin = gameMethods.GetGameByKey(gamekey, out error);

            if (gameToJoin == null)
            {
                ViewBag.Error = "Game not found.";
                return View();
            }
            if (gameToJoin.PlayerBlackId != 0)
            {
                ViewBag.Error = "Game is already full.";
                return View();
            }

            gameMethods.UpdateGame(gameToJoin, out error);
            return RedirectToAction(gamekey);
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
            string error = "";
            gameDetails= gameMethods.GetGameByKey(gameKey, out  error);
            return View(gameDetails);
        }

    }
}

