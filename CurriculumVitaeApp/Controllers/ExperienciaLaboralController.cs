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
        [ValidateAntiForgeryToken]
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
            return View(experienciaLaboral);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(string encryptedId, [Bind("FechaInicio,FechaTermino,Empresa,Descripcion")] ExperienciaLaboral antecedenteLaboral)
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

            antecedenteLaboral.Vigente = false;

            if (antecedenteLaboral.FechaTermino == null)
            {
                antecedenteLaboral.Vigente = true;
            }

            antecedenteLaboral.Id = realId;
            antecedenteLaboral.UsuarioID = idUsuario;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(antecedenteLaboral);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExperienciaLaboralExists(antecedenteLaboral.Id))
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

            return View(antecedenteLaboral);
        }

        // POST: ExperienciaLaboral/Delete/5
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
