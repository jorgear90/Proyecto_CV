using System.Diagnostics;
using CurriculumVitaeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurriculumVitaeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        //Get: pagina principal
        public IActionResult Index()
        {
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                // No hay token → redirigir
                return RedirectToAction("Login", "Usuarios");
            }

            // Ruta física a wwwroot/cv
            var rutaCarpeta = Path.Combine(_env.WebRootPath, "cv");

            // Obtiene todos los archivos de imagen en la carpeta
            var imagenes = Directory.GetFiles(rutaCarpeta)
                                    .Select(Path.GetFileName)   // solo el nombre del archivo
                                    .ToList();

            return View(imagenes);

        }

        //Post parav eliminar el token y cerrar sesión
        [HttpPost]
        public IActionResult Logout()
        {
            // Eliminar todas las cookies de sesión/auth
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie.StartsWith(".AspNetCore.") ||
                    cookie.Contains("Auth") ||
                    cookie.Contains("Token") ||
                    cookie == "jwt" ||
                    cookie == "access_token")
                {
                    Response.Cookies.Delete(cookie);
                }
            }

            // Opcional: Eliminar cookie específica si sabes su nombre exacto
            Response.Cookies.Delete("jwt"); // o el nombre que uses para tu token
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete(".AspNetCore.Session");

            // Limpiar la sesión
            //HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
