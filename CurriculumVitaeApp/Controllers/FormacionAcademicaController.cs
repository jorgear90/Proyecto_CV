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
using CurriculumVitaeApp.Migrations;

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

            ViewData["TipoInstitucionID"] = new SelectList(_context.TipoInstitucion, "ID", "Tipo");

            return View(await antecedentesAcademicos.ToListAsync());
        }

        // GET: FormacionAcademica/Create
        public async Task<IActionResult> Crear()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            ViewBag.idUsuario = idUsuario;
            ViewData["TipoInstitucionID"] = new SelectList(_context.TipoInstitucion, "ID", "Tipo");

            return View();
        }

        // POST: FormacionAcademica/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Crear([Bind("Id,UsuarioID,TipoInstitucionID,NombreInstitucion,Ciudad,Carrera,AnhoInicio,AnhoTermino,Vigente,Descripcion")] CurriculumVitaeApp.Models.FormacionAcademica formacionAcademica)
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
            ViewData["TipoInstitucionID"] = new SelectList(_context.TipoInstitucion, "ID", "Tipo", formacionAcademica.TipoInstitucionID);
            return View(formacionAcademica);
        }

        // POST: FormacionAcademica/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Editar(string idEditar, [Bind("TipoInstitucionID,NombreInstitucion,Ciudad,Carrera,AnhoInicio,AnhoTermino,Descripcion")] CurriculumVitaeApp.Models.FormacionAcademica formacionAcademica)
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

            // Obtener el registro original (ya trackeado)
            var registroExistente = await _context.FormacionAcademica
                .FirstOrDefaultAsync(a => a.Id == realId);

            if (registroExistente == null)
                return NotFound();

            // Lógica de reemplazo según si la descripción viene nula o no
            if (formacionAcademica.Descripcion == null)
            {
                // Mantener la descripción original
                formacionAcademica.Descripcion = registroExistente.Descripcion;
                ModelState.Remove("Descripcion");
            }
            else
            {
                // Si cambia la descripción, NO permitir cambiar otros campos
                formacionAcademica.AnhoInicio = registroExistente.AnhoInicio;
                formacionAcademica.AnhoTermino = registroExistente.AnhoTermino;
                formacionAcademica.Carrera = registroExistente.Carrera;
                formacionAcademica.Ciudad = registroExistente.Ciudad;
                formacionAcademica.NombreInstitucion = registroExistente.NombreInstitucion;
                formacionAcademica.TipoInstitucionID = registroExistente.TipoInstitucionID;
                ModelState.Remove("AnhoInicio");
                ModelState.Remove("NombreInstitucion");
                ModelState.Remove("Carrera");
                ModelState.Remove("Ciudad");
            }

            if (!ModelState.IsValid)
                return View(nameof(Index));

            registroExistente.AnhoInicio = formacionAcademica.AnhoInicio;
            registroExistente.AnhoTermino = formacionAcademica.AnhoTermino;
            registroExistente.NombreInstitucion = formacionAcademica.NombreInstitucion;
            registroExistente.Ciudad = formacionAcademica.Ciudad;
            registroExistente.Descripcion = formacionAcademica.Descripcion;
            registroExistente.TipoInstitucionID = formacionAcademica.TipoInstitucionID;
            registroExistente.UsuarioID = idUsuario;

            // Actualizar campo Vigente
            bool vigente = formacionAcademica.AnhoTermino == null;
            registroExistente.Vigente = vigente;

            // Guardar cambios
            await _context.SaveChangesAsync();

            ViewData["TipoInstitucionID"] = new SelectList(_context.TipoInstitucion, "ID", "Tipo", formacionAcademica.TipoInstitucionID);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditarDescripcion(string idEditar, string? Descripcion)
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

            // Obtener el registro original (ya trackeado)
            var registroExistente = await _context.FormacionAcademica
                .FirstOrDefaultAsync(a => a.Id == realId);

            if (registroExistente == null)
                return NotFound();

            //Editar descripción:
            registroExistente.Descripcion = Descripcion;

            // Guardar cambios
            await _context.SaveChangesAsync();

            //ViewData["TipoInstitucionID"] = new SelectList(_context.TipoInstitucion, "ID", "Tipo", formacionAcademica.TipoInstitucionID);

            return RedirectToAction(nameof(Index));
        }

        // POST: FormacionAcademica/Delete/5
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

            var formacionAcademica = await _context.FormacionAcademica.FindAsync(realId);
            if (formacionAcademica != null)
            {
                _context.FormacionAcademica.Remove(formacionAcademica);
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

        private bool FormacionAcademicaExists(int id)
        {
            return _context.FormacionAcademica.Any(e => e.Id == id);
        }
    }
}
