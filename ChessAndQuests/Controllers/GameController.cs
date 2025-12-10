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
    }
}
