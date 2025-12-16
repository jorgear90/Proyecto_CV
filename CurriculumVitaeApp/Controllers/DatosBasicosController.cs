using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurriculumVitaeApp.Data;
using CurriculumVitaeApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CurriculumVitaeApp.Helpers;

namespace CurriculumVitaeApp.Controllers
{
    public class DatosBasicosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IdProtector _idProtector;

        public DatosBasicosController(AppDbContext context, IdProtector idProtector)
        {
            _context = context;
            _idProtector = idProtector;
        }

        // GET: DatosBasicos
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

            var datosBasicos = _context.DatosBasicos.Include(d => d.Usuarios).Where(d => d.UsuarioID == idUsuario);
            return View(await datosBasicos.ToListAsync());
        }

        // GET: DatosBasicos/Create
        public async Task<IActionResult> Crear()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            return View();
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

        // POST: DatosBasicos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Crear([Bind("Id,UsuarioID,NombreDato,ValorDato")] DatosBasicos datosBasicos)
        {
            var idUsuario = await getIdUsuario();

            datosBasicos.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                _context.Add(datosBasicos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(datosBasicos);
        }

        // POST: DatosBasicos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Editar(string idEditar, [Bind("NombreDato,ValorDato")] DatosBasicos datosBasicos)
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

            datosBasicos.Id = realId;
            datosBasicos.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                try
                {
                    var registroEditado = await _context.DatosBasicos.Where(p => p.Id == datosBasicos.Id).FirstOrDefaultAsync();

                    registroEditado.NombreDato = datosBasicos.NombreDato;
                    registroEditado.ValorDato = datosBasicos.ValorDato;

                    _context.Update(registroEditado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatosBasicosExists(datosBasicos.Id))
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
            return View("Index", datosBasicos);
        }

        // POST: DatosBasicos/Delete/5
        [HttpPost, ActionName("Eliminar")]
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

            var datosBasicos = await _context.DatosBasicos.FindAsync(realId);
            if (datosBasicos != null)
            {
                _context.DatosBasicos.Remove(datosBasicos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatosBasicosExists(int id)
        {
            return _context.DatosBasicos.Any(e => e.Id == id);
        }
    }
}
