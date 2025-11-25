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
    public class HabilidadesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IdProtector _idProtector;

        public HabilidadesController(AppDbContext context, IdProtector idProtector)
        {
            _context = context;
            _idProtector = idProtector;
        }

        // GET: Habilidades
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

            var habilidades = _context.Habilidades.Include(d => d.Usuarios).Where(d => d.UsuarioID == idUsuario);

            return View(await habilidades.ToListAsync());
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

        // POST: Habilidades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Id,UsuarioID,Descripcion")] Habilidad habilidad)
        {
            var idUsuario = await getIdUsuario();

            habilidad.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                _context.Add(habilidad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioID"] = new SelectList(_context.Usuarios, "Id", "Id", habilidad.UsuarioID);
            return View(habilidad);
        }

        // GET: Habilidades/Edit/5
        public async Task<IActionResult> Editar(string id)
        {
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            int realId;

            try
            {
                realId = _idProtector.DecryptId(id);
            }
            catch
            {
                return BadRequest("ID inválido");
            }

            var habilidad = await _context.Habilidades.FindAsync(realId);
            if (habilidad == null)
            {
                return NotFound();
            }

            ViewBag.EncryptedId = id;

            return View(habilidad);
        }

        // POST: Habilidades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(string encryptedId, [Bind("Descripcion")] Habilidad habilidad)
        {
            var idUsuario = await getIdUsuario();

            int realId;

            try
            {
                realId = _idProtector.DecryptId(encryptedId);
            }
            catch
            {
                return BadRequest("ID inválido");
            }

            habilidad.Id = realId;
            habilidad.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habilidad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabilidadExists(habilidad.Id))
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

            return View(habilidad);
        }

        // POST: Habilidades/Delete/5
        [HttpPost, ActionName("Delete")]
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

            var habilidad = await _context.Habilidades.FindAsync(realId);
            if (habilidad != null)
            {
                _context.Habilidades.Remove(habilidad);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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

        private bool HabilidadExists(int id)
        {
            return _context.Habilidades.Any(e => e.Id == id);
        }
    }
}
