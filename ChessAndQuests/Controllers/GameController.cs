using Microsoft.AspNetCore.Mvc;

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
        public IActionResult CreateGame(string gamekey, int playerId)
        {
            //en funktion som tar in indata och skapar ett nytt spel och returnerar ett GameDetails objekt
            //Skapa nytt spel i dal GameMethods.CreateGame(GameDetails)
            //returnera till spelbrädssidan med det nya spelet
        }
    }
}
