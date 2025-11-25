using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CurriculumVitaeApp.Data;
using CurriculumVitaeApp.Models;
using Microsoft.AspNetCore.Identity;
using Humanizer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CurriculumVitaeApp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;
        private readonly IConfiguration _config;

        public UsuariosController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
            _config = config;
        }

        // GET: Usuarios/Login
        public async Task<IActionResult> Login()
        {
            var token = Request.Cookies["jwtToken"];

            if (!string.IsNullOrEmpty(token))
            {
                // No hay token → redirigir
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Post: Autentificación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Correo,Password")] AuthDto usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioExiste = await _context.Usuarios.Where(u => u.Correo == usuario.Correo).FirstOrDefaultAsync();

                if (usuarioExiste == null) {
                    ViewBag.Mensaje = "Usuario o contraseña inválidos.";
                    return View();
                }

                var password = await _context.Usuarios.Where(u => u.Password == usuarioExiste.Password).Select(u => u.Password).FirstOrDefaultAsync();

                var result = _passwordHasher.VerifyHashedPassword(usuarioExiste, usuarioExiste.Password, usuario.Password);

                if (password == null || result == PasswordVerificationResult.Failed) {
                    ViewBag.Mensaje = "Usuario o contraseña inválidos.";
                    return View();
                }

                var token = GenerarJwtToken(usuario.Correo);

                Response.Cookies.Append("jwtToken", token, new CookieOptions
                {
                    HttpOnly = true, // evita acceso desde JS
                    Secure = false, //
                    SameSite = SameSiteMode.Lax, //Mas flexible
                    Expires = DateTime.UtcNow.AddHours(2)
                });

                //ViewBag.Mensaje = "Usuario valido.";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Mensaje = "Modelo invalido.";
            return View();
        }

        private string GenerarJwtToken(string correo)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, correo),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class AuthDto
        {
            public string Correo { get; set; }
            public string Password { get; set; }
        }

        public class CreateUsuarioDto
        {
            public string Correo { get; set; }
            public string Password { get; set; }
            public string PasswordRepetido { get; set; }
        }

        // GET: Usuarios/Create
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Correo,Password,PasswordRepetido")] CreateUsuarioDto usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.Password != usuario.PasswordRepetido) { 
                    ViewBag.Mensaje = "El password no coincide.";
                    return View();
                }

                var usuarioExiste = await _context.Usuarios.Where(u => u.Correo == usuario.Correo).FirstOrDefaultAsync();

                if (usuarioExiste != null)
                {
                    ViewBag.Mensaje = "Ya existe un registro para este correo";
                    return View();
                }

                var crearUsuario = new Usuario { Correo = usuario.Correo, Password = usuario.Password };
                crearUsuario.Password = _passwordHasher.HashPassword(crearUsuario ,usuario.Password);

                _context.Add(crearUsuario);
                await _context.SaveChangesAsync();

                await CrearDatosBasicosPorDefecto(crearUsuario.Id);

                TempData["Creado"] = true;

                return RedirectToAction("Create");
            }
            ViewBag.Mensaje = "Error al crear al usuario.";
            return View();
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("Id,Correo,Password")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        //Método que crea registros sugridos
        private async Task CrearDatosBasicosPorDefecto(int usuarioId)
        {
            var registrosPorDefecto = new List<DatosBasicos>
            {
                new DatosBasicos
                {
                    UsuarioID = usuarioId,
                    Nombre = "Nombre",
                    Valor = "Tu nombre"
                },
                new DatosBasicos
                {
                    UsuarioID = usuarioId,
                    Nombre = "Apellido",
                    Valor = "Tu apellido"
                },
                new DatosBasicos
                {
                    UsuarioID = usuarioId,
                    Nombre = "Número",
                    Valor = "Tu número de teléfono"
                }
            };

            _context.AddRange(registrosPorDefecto);
            await _context.SaveChangesAsync();
        }


        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
