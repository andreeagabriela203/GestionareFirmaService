using FirmaService.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirmaService.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]  // Cererea GET pentru vizualizarea formularului de login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]  // Cererea POST pentru procesarea autentificării
        public IActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username == "Andreea" && model.Password == "Narcise21")
                {
                    TempData["Loggedin"] = true;
                    TempData["Message"] = "Autentificare reusita!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Loggedin"] = false;
                    TempData["Message"] = "Autentificare esuata";
                }
            }
            return View(model);
        } 
        public IActionResult Logout()
        {
        TempData.Remove("Loggedin");
        TempData["Message"] = "Te-ai deconectat!";
        return RedirectToAction("Login", "Account");
          }
    }
  

}
