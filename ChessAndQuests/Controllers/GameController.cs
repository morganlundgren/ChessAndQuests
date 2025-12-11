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
                newGame.PlayerBlackId = 0;
                newGame.GameKey = gamekey;
                newGame.CurrentFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\r\n";
                newGame.status = 0;
                newGame.turnId = playerId;
            }
            string error = "";

            gameMethods.CreateGame(newGame, out error);
           
            return RedirectToAction("Play", "GameBoard", new { gameId = newGame.GameId });
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
            GameDetails gameToJoin = gameMethods.GetGameByKey(gamekey);

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
            gameToJoin.PlayerBlackId = playerId;
            gameMethods.UpdateGame(gameToJoin);
            return RedirectToAction("Play", "GameBoard", new { gameId = gameToJoin.GameId });
        }



    }
}

