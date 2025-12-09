using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CurriculumVitaeApp.Data;
using CurriculumVitaeApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CurriculumVitaeApp.Helpers;

namespace CurriculumVitaeApp.Controllers
{
    public class ExperienciaLaboralController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IdProtector _idProtector;

        public ExperienciaLaboralController(AppDbContext context, IdProtector idProtector)
        {
            _context = context;
            _idProtector = idProtector;
        }

        // GET: ExperienciaLaboral
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

            var antecedentesLaborales = _context.AntecedentesLaborales.Include(d => d.Usuarios).Where(d => d.UsuarioID == idUsuario);

            return View(await antecedentesLaborales.ToListAsync());
        }

        // GET: ExperienciaLaboral/Create
        public async Task<IActionResult> Crear()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            ViewBag.idUsuario = idUsuario;
            return View();
        }

        // POST: ExperienciaLaboral/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Crear([Bind("Id,UsuarioID,FechaInicio,FechaTermino,Empresa,Descripcion")] ExperienciaLaboral experienciaLaboral)
        {
            var idUsuario = await getIdUsuario();

            experienciaLaboral.Vigente = false;

            if (experienciaLaboral.FechaTermino == null)
            {
                experienciaLaboral.Vigente = true;
            }

            experienciaLaboral.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                _context.Add(experienciaLaboral);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("Index", experienciaLaboral);
        }

        // GET: ExperienciaLaboral/Edit/5
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

            var antecedenteLaboral = await _context.AntecedentesLaborales.FindAsync(realId);
            if (antecedenteLaboral == null) return NotFound();

            ViewBag.EncryptedId = id;

            return View(antecedenteLaboral);
        }

        // POST: ExperienciaLaboral/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Editar(string idEditar, [Bind("FechaInicio,FechaTermino,Empresa,Descripcion")] ExperienciaLaboral antecedenteLaboral)
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
            var registroExistente = await _context.AntecedentesLaborales
                .FirstOrDefaultAsync(a => a.Id == realId);

            if (registroExistente == null)
                return NotFound();

            // Lógica de reemplazo según si la descripción viene nula o no
            if (antecedenteLaboral.Descripcion == null)
            {
                // Mantener la descripción original
                antecedenteLaboral.Descripcion = registroExistente.Descripcion;
                ModelState.Remove("Descripcion");
            }
            else
            {
                // Si cambia la descripción, NO permitir cambiar otros campos
                antecedenteLaboral.FechaInicio = registroExistente.FechaInicio;
                antecedenteLaboral.FechaTermino = registroExistente.FechaTermino;
                antecedenteLaboral.Empresa = registroExistente.Empresa;
                ModelState.Remove("FechaInicio");
                ModelState.Remove("Empresa");
            }

            if (!ModelState.IsValid)
                return PartialView("Index", antecedenteLaboral);

            // ------------------------
            // 🔥 ACTUALIZAR SOLO EL REGISTRO TRACKEADO
            // ------------------------

            registroExistente.FechaInicio = antecedenteLaboral.FechaInicio;
            registroExistente.FechaTermino = antecedenteLaboral.FechaTermino;
            registroExistente.Empresa = antecedenteLaboral.Empresa;
            registroExistente.Descripcion = antecedenteLaboral.Descripcion;
            registroExistente.UsuarioID = idUsuario;

            // Actualizar campo Vigente
            bool vigente = antecedenteLaboral.FechaTermino == null;
            registroExistente.Vigente = vigente;

            // Guardar cambios
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: ExperienciaLaboral/Delete/5
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

            var antecedenteLaboral = await _context.AntecedentesLaborales.FindAsync(realId);
            if (antecedenteLaboral != null)
            {
                _context.AntecedentesLaborales.Remove(antecedenteLaboral);
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


        private bool ExperienciaLaboralExists(int id)
        {
            return _context.AntecedentesLaborales.Any(e => e.Id == id);
        }
    }
}
