using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurriculumVitaeApp.Data;
using CurriculumVitaeApp.Models;
using CurriculumVitaeApp.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CurriculumVitaeApp.Controllers
{
    public class ConocimientosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IdProtector _idProtector;

        public ConocimientosController(AppDbContext context, IdProtector idProtector)
        {
            _context = context;
            _idProtector = idProtector;
        }

        // GET: Conocimientos
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Decodificar el token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Obtener el claim del correo
            var correo = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var idUsuario = await _context.Usuarios.Where(u => u.Correo == correo).Select(u => u.Id).FirstOrDefaultAsync();

            var conocimientos = _context.Conocimientos.Include(d => d.Usuarios).Where(d => d.UsuarioID == idUsuario);

            return View(await conocimientos.ToListAsync());
        }

        // GET: Habilidades/Create
        public async Task<IActionResult> Crear()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            ViewBag.idUsuario = idUsuario;
            return View();
        }

        // POST: Conocimientos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Id,UsuarioID,Descripcion")] Conocimiento conocimiento)
        {
            var idUsuario = await getIdUsuario();

            conocimiento.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                _context.Add(conocimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioID"] = new SelectList(_context.Usuarios, "Id", "Id", conocimiento.UsuarioID);
            return View(conocimiento);
        }

        // POST: Conocimientos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Editar(string idEditar, [Bind("Descripcion")] Conocimiento conocimiento)
        {
            var idUsuario = await getIdUsuario();

            int realId;

            try
            {
                realId = _idProtector.DecryptId(idEditar);
            }
            catch
            {
                return BadRequest("ID inválido");
            }

            conocimiento.Id = realId;
            conocimiento.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conocimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConocimientoExists(conocimiento.Id))
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

            return View("Index", conocimiento);
        }

        // POST: Conocimientos/Delete/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            int realId;

            try
            {
                realId = _idProtector.DecryptId(id);
            }
            catch
            {
                return BadRequest("ID inválido");
            }

            var conocimiento = await _context.Conocimientos.FindAsync(realId);
            if (conocimiento != null)
            {
                _context.Conocimientos.Remove(conocimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Este método permite obtener el id del usuario por medio del token
        public async Task<int> getIdUsuario()
        {
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return 0;
            }

            // Decodificar el token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Obtener el claim del correo
            var correo = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var idUsuario = await _context.Usuarios.Where(u => u.Correo == correo).Select(u => u.Id).FirstOrDefaultAsync();

            return idUsuario;

        }

        private bool ConocimientoExists(int id)
        {
            return _context.Conocimientos.Any(e => e.Id == id);
        }
    }
}
