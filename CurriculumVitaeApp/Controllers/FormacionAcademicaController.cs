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
    public class FormacionAcademicaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IdProtector _idProtector;

        public FormacionAcademicaController(AppDbContext context, IdProtector idProtector)
        {
            _context = context;
            _idProtector = idProtector;
        }

        // GET: FormacionAcademica
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

            var antecedentesAcademicos = _context.FormacionAcademica.Include(d => d.TipoInstitucion).Where(d => d.UsuarioID == idUsuario);

            return View(await antecedentesAcademicos.ToListAsync());
        }

        // GET: FormacionAcademica/Create
        public async Task<IActionResult> Crear()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            ViewBag.idUsuario = idUsuario;
            ViewData["TipoInstitucionID"] = new SelectList(_context.tipoInstitucion, "ID", "Tipo");

            return View();
        }

        // POST: FormacionAcademica/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Id,UsuarioID,TipoInstitucionID,NombreInstitucion,Carrera,AnhoInicio,AnhoTermino,Vigente,Descripcion")] FormacionAcademica formacionAcademica)
        {
            var idUsuario = await getIdUsuario();

            formacionAcademica.Vigente = false;

            if (formacionAcademica.AnhoTermino == null)
            {
                formacionAcademica.Vigente = true;
            }

            formacionAcademica.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                _context.Add(formacionAcademica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoInstitucionID"] = new SelectList(_context.tipoInstitucion, "ID", "Tipo", formacionAcademica.TipoInstitucionID);
            return View(formacionAcademica);
        }

        // GET: FormacionAcademica/Edit/5
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

            var formacionAcademica = await _context.FormacionAcademica.FindAsync(realId);
            if (formacionAcademica == null)
            {
                return NotFound();
            }

            ViewBag.EncryptedId = id;
            ViewData["TipoInstitucionID"] = new SelectList(_context.tipoInstitucion, "ID", "Tipo", formacionAcademica.TipoInstitucionID);

            return View(formacionAcademica);
        }

        // POST: FormacionAcademica/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(string encryptedId, [Bind("TipoInstitucionID,NombreInstitucion,Carrera,AnhoInicio,AnhoTermino,Descripcion")] FormacionAcademica formacionAcademica)
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

            formacionAcademica.Vigente = false;

            if (formacionAcademica.AnhoTermino == null)
            {
                formacionAcademica.Vigente = true;
            }

            formacionAcademica.Id = realId;
            formacionAcademica.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formacionAcademica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormacionAcademicaExists(formacionAcademica.Id))
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

            ViewData["TipoInstitucionID"] = new SelectList(_context.tipoInstitucion, "ID", "Tipo", formacionAcademica.TipoInstitucionID);
            return View(formacionAcademica);
        }

        // POST: FormacionAcademica/Delete/5
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

            var formacionAcademica = await _context.FormacionAcademica.FindAsync(realId);
            if (formacionAcademica != null)
            {
                _context.FormacionAcademica.Remove(formacionAcademica);
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

        private bool FormacionAcademicaExists(int id)
        {
            return _context.FormacionAcademica.Any(e => e.Id == id);
        }
    }
}
