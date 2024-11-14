using Microsoft.AspNetCore.Mvc;
using PescaSystem.Services;

namespace PescaSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public AuthController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string nombreUsuario, string password)
        {
            var usuario = _usuarioService.ValidarUsuario(nombreUsuario, password);

            if (usuario != null)
            {
                HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                return RedirectToAction("Index", "PescaLog");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UsuarioId");
            return RedirectToAction("Login");
        }
    }

}
