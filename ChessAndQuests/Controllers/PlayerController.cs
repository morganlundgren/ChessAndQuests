using ChessAndQuests.DAL;
using ChessAndQuests.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChessAndQuests.Controllers
{
    public class PlayerController : Controller
    {
        [HttpGet]
        public IActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        public IActionResult SignUp(PlayerDetails playerDetails)
        {
            PlayerMethods playerMethods = new PlayerMethods();
            int i = 0;
            string error = "";

            i = playerMethods.CreatePlayer(playerDetails, out error);

            ViewBag.errorSignUp = error;

            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult Signin()
        {
            return View();

        }
        [HttpPost]
        public IActionResult SignIn(PlayerDetails playerDetails)
        {
            PlayerMethods playerMethods = new PlayerMethods();
            string error = "";

            return RedirectToAction("Index", "Home");
        }


    }
}
