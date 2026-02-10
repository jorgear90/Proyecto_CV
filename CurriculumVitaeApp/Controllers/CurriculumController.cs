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
using System.Drawing;
using System.Xml.Linq;
using CurriculumVitaeApp.Helpers;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace CurriculumVitaeApp.Controllers
{
    public class CurriculumController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IdProtector _idProtector;
        private readonly IWebHostEnvironment _env;

        public CurriculumController(AppDbContext context, IdProtector idProtector, IWebHostEnvironment env)
        {
            _context = context;
            _idProtector = idProtector;
            _env = env;
        }

        public async Task<IActionResult> MisCurriculums()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var curriculums = await _context.Curriculum
                .Where(c => c.UsuarioID == idUsuario)
                .OrderBy(c => c.Fecha)
                .ToListAsync();

            return View(curriculums);
        }

        // Método GET para la vista parcial DatosBasicos
        public async Task<IActionResult> SelectorDatosBasicos()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var curriculumId = await _context.Curriculum
                .Where(c => c.UsuarioID == idUsuario)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            var idsSeleccionados = new HashSet<int>();

            if (curriculumId != 0) 
            {
                idsSeleccionados = await _context.CurriculumSeleccion
                    .Where(cs => cs.CurriculumID == curriculumId && cs.TipoDatoCurriculumID == 1)
                    .Select(cs => cs.TipoDatoID) 
                    .ToHashSetAsync();
            }

            ViewBag.IdsSeleccionados = idsSeleccionados;

            var datosBasicos = _context.DatosBasicos
                .Include(d => d.Usuarios)
                .Where(d => d.UsuarioID == idUsuario);

            return View(await datosBasicos.ToListAsync());
        }

        //Método GET para la vista parcial SelectorHabilidad
        public async Task<IActionResult> SelectorHabilidad()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var curriculumId = await _context.Curriculum
                .Where(c => c.UsuarioID == idUsuario)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            var idsSeleccionados = new HashSet<int>();

            if (curriculumId != 0) 
            {
                idsSeleccionados = await _context.CurriculumSeleccion
                    .Where(cs => cs.CurriculumID == curriculumId && cs.TipoDatoCurriculumID == 2)
                    .Select(cs => cs.TipoDatoID) 
                    .ToHashSetAsync(); 
            }

            ViewBag.IdsSeleccionados = idsSeleccionados;

            var habilidades = _context.Habilidades
                .Include(d => d.Usuarios)
                .Where(d => d.UsuarioID == idUsuario);

            return View(await habilidades.ToListAsync());
        }

        //Método GET para la vista parcial SelectorConocimiento
        public async Task<IActionResult> SelectorConocimiento()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var curriculumId = await _context.Curriculum
                .Where(c => c.UsuarioID == idUsuario)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            var idsSeleccionados = new HashSet<int>();

            if (curriculumId != 0) 
            {
                idsSeleccionados = await _context.CurriculumSeleccion
                    .Where(cs => cs.CurriculumID == curriculumId && cs.TipoDatoCurriculumID == 3)
                    .Select(cs => cs.TipoDatoID) 
                    .ToHashSetAsync(); 
            }

            ViewBag.IdsSeleccionados = idsSeleccionados;

            var conocimientos = _context.Conocimientos
                .Include(d => d.Usuarios)
                .Where(d => d.UsuarioID == idUsuario);

            return View(await conocimientos.ToListAsync());
        }

        //Método GET para la vista parcial SelectorFormacionAcademica
        public async Task<IActionResult> SelectorFormacionAcademica()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var curriculumId = await _context.Curriculum
                .Where(c => c.UsuarioID == idUsuario)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            var idsSeleccionados = new HashSet<int>();

            if (curriculumId != 0)
            {
                idsSeleccionados = await _context.CurriculumSeleccion
                    .Where(cs => cs.CurriculumID == curriculumId && cs.TipoDatoCurriculumID == 5)
                    .Select(cs => cs.TipoDatoID)
                    .ToHashSetAsync();
            }

            ViewBag.IdsSeleccionados = idsSeleccionados;

            var antecedentesAcademicos = _context.FormacionAcademica
                .Include(d => d.Usuarios)
                .Where(d => d.UsuarioID == idUsuario);

            ViewData["TipoInstitucionID"] = new SelectList(_context.TipoInstitucion, "ID", "Tipo");

            return View(await antecedentesAcademicos.ToListAsync());
        }

        //Método GET para la vista parcial SelectorExperienciaLaboral
        public async Task<IActionResult> SelectorExperienciaLaboral()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var curriculumId = await _context.Curriculum
                .Where(c => c.UsuarioID == idUsuario)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            var idsSeleccionados = new HashSet<int>();

            if (curriculumId != 0)
            {
                idsSeleccionados = await _context.CurriculumSeleccion
                    .Where(cs => cs.CurriculumID == curriculumId && cs.TipoDatoCurriculumID == 4)
                    .Select(cs => cs.TipoDatoID)
                    .ToHashSetAsync();
            }

            ViewBag.IdsSeleccionados = idsSeleccionados;

            var antecedentesLaborales = _context.ExperienciaLaboral
                .Include(d => d.Usuarios)
                .Where(d => d.UsuarioID == idUsuario);

            return View(await antecedentesLaborales.ToListAsync());
        }

        // Método GET para la vista parcial DatosBasicos
        public async Task<IActionResult> SelectorEnlaces()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var curriculumId = await _context.Curriculum
                .Where(c => c.UsuarioID == idUsuario)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            var idsSeleccionados = new HashSet<int>();

            if (curriculumId != 0)
            {
                idsSeleccionados = await _context.CurriculumSeleccion
                    .Where(cs => cs.CurriculumID == curriculumId && cs.TipoDatoCurriculumID == 6)
                    .Select(cs => cs.TipoDatoID)
                    .ToHashSetAsync();
            }

            ViewBag.IdsSeleccionados = idsSeleccionados;

            var enlaces = _context.Enlaces
                .Include(d => d.Usuarios)
                .Where(d => d.UsuarioID == idUsuario);

            return View(await enlaces.ToListAsync());
        }

        // GET: Curriculum/Details/5
        public async Task<IActionResult> VistaSeleccionar()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var encabezado = await _context.Curriculum.Where( e => e.UsuarioID == idUsuario).Select( e => e.Encabezado).FirstOrDefaultAsync();
            var nombre = await _context.Curriculum.Where(e => e.UsuarioID == idUsuario).Select(e => e.Nombre).FirstOrDefaultAsync();

            ViewBag.Encabezado = encabezado;
            ViewBag.NombreCv = nombre;

            return View();
        }

        //Función que permite descargar un cv desde la vista misCurriculums
        [HttpGet]
        public async Task<IActionResult> DescargarCv(string idDescargar)
        {
            // id = idCv
            var idUsuario = await getIdUsuario();

            int realId;

            try
            {
                realId = _idProtector.DecryptId(idDescargar);
            }
            catch
            {
                return BadRequest("ID inválido");
            }

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            // Ruta física a wwwroot
            var webRoot = _env.WebRootPath;

            // wwwroot/cv-usuarios/{idUsuario}/{idCv}.pdf
            var rutaArchivo = Path.Combine(
                webRoot,
                "cv-usuarios",
                idUsuario.ToString(),
                $"{realId}.pdf"
            );

            // Verificar que el archivo exista
            if (!System.IO.File.Exists(rutaArchivo))
            {
                return NotFound("El curriculum solicitado no existe.");
            }

            //Obtener nombre del archivo
            var nombrePdf = await _context.Curriculum.Where(c => c.Id == realId).Select(c => c.Nombre).FirstOrDefaultAsync();

            // Leer bytes
            var fileBytes = await System.IO.File.ReadAllBytesAsync(rutaArchivo);

            // Descargar archivo
            return File(
                fileBytes,
                "application/pdf",
                $"{nombrePdf}.pdf"
            );
        }

        //Método que permite eliminar un cv
        [HttpPost, ActionName("EliminarCv")]
        public async Task<IActionResult> DeleteConfirmed(string idEliminar)
        {
            int realId;

            try
            {
                realId = _idProtector.DecryptId(idEliminar);
            }
            catch
            {
                return BadRequest("ID inválido");
            }

            var idUsuario = await getIdUsuario();
            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            // Buscar curriculum en BD
            var curriculum = await _context.Curriculum
                .FirstOrDefaultAsync(c => c.Id == realId && c.UsuarioID == idUsuario);

            if (curriculum == null)
                return NotFound();

            // 👉 Eliminar archivo físico
            var rutaArchivo = Path.Combine(
                _env.WebRootPath,
                "cv-usuarios",
                idUsuario.ToString(),
                $"{realId}.pdf"
            );

            if (System.IO.File.Exists(rutaArchivo))
            {
                System.IO.File.Delete(rutaArchivo);
            }

            var carpetaUsuario = Path.GetDirectoryName(rutaArchivo);

            if (Directory.Exists(carpetaUsuario) &&
                !Directory.EnumerateFileSystemEntries(carpetaUsuario).Any())
            {
                Directory.Delete(carpetaUsuario);
            }

            // 👉 Eliminar registro BD
            _context.Curriculum.Remove(curriculum);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MisCurriculums));
        }

        //Este método muestra una vista previa del cv
        [HttpGet]
        public async Task<IActionResult> VistaPreviaCv(string id)
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

            var idUsuario = await getIdUsuario();
            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            // Verificar que el CV pertenezca al usuario
            var existe = await _context.Curriculum
                .AnyAsync(c => c.Id == realId && c.UsuarioID == idUsuario);

            if (!existe)
                return Forbid();

            // Ruta física del PDF
            var rutaArchivo = Path.Combine(
                _env.WebRootPath,
                "cv-usuarios",
                idUsuario.ToString(),
                $"{realId}.pdf"
            );

            if (!System.IO.File.Exists(rutaArchivo))
                return NotFound("El archivo no existe.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(rutaArchivo);

            // 👀 Vista previa (inline)
            return File(fileBytes, "application/pdf");
        }


        //Este método recepciona y entrega los datos seleccionados para generar el curriculum
        [HttpPost]
        public async Task<IActionResult> GenerarPDF(string seleccionadosJson, string encabezado, string nombreCv)
        {
            if (string.IsNullOrEmpty(seleccionadosJson))
            {
                return BadRequest("El JSON llegó vacío o nulo.");
            }

            var seleccionados = JsonConvert.DeserializeObject<List<ItemSeleccion>>(seleccionadosJson ?? "[]");


            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var cantidadCv = await _context.Curriculum.CountAsync(u => u.UsuarioID == idUsuario);

            if (cantidadCv >= 5)
            {
                TempData["SwalError"] = "Has alcanzado el límite máximo de 5 currículums. Para crear uno nuevo, elimina alguno de los existentes.";
                return RedirectToAction("VistaSeleccionar"); 
            }


            //int curriculumId = await _context.Curriculum.Where( c => c.UsuarioID == idUsuario).Select(c => c.Id).FirstOrDefaultAsync();

            //int valorId = await _context.Encabezados.Where( e => e.UsuarioID == idUsuario).Select(e => e.Id).FirstOrDefaultAsync();

            var nuevoCv = new Curriculum
            {
                UsuarioID = idUsuario,
                Nombre = nombreCv,
                Encabezado = encabezado,
                Fecha = DateOnly.FromDateTime(DateTime.Today),
                
                //Orden = index++
            };
            _context.Add(nuevoCv);
            await _context.SaveChangesAsync();

            var curriculumId = nuevoCv.Id;

            /*
            if (curriculumId == 0)
            {
                var nuevoCv = new Curriculum
                {
                    UsuarioID = idUsuario,
                    Nombre = nombreCv,
                    Encabezado = encabezado,
                    //Orden = index++
                };
                _context.Add(nuevoCv);
                await _context.SaveChangesAsync();

                curriculumId = nuevoCv.Id;
            }
            else
            {
                var cv = await _context.Curriculum.Where(c => c.Id == curriculumId).FirstOrDefaultAsync();

                cv.Nombre = nombreCv;
                cv.Encabezado = encabezado;

                _context.Update(cv);
                await _context.SaveChangesAsync();
            }*/

            /*if (valorId == 0)
            {
                var nuevoEncabezado = new EncabezadoCurriculum
                {
                    UsuarioID = idUsuario,
                    ValorEncabezado = valor
                    //Orden = index++
                };
                _context.Add(nuevoEncabezado);
                await _context.SaveChangesAsync();
            }
            /*else
            {
                var encabezadoExiste = await _context.Encabezados.Where(e => e.UsuarioID == idUsuario).FirstOrDefaultAsync();
                encabezadoExiste.ValorEncabezado = valor;

                _context.Update(encabezadoExiste);
                await _context.SaveChangesAsync();
            }*/

            int index = 0;

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var antiguos = _context.CurriculumSeleccion.Where(c => c.CurriculumID == curriculumId);
                _context.RemoveRange(antiguos);
                await _context.SaveChangesAsync();

                foreach (var s in seleccionados)
                {
                    var tipoDatoCurriculumID = int.Parse(s.Tipo);
                    int realId = _idProtector.DecryptId(s.Id);

                    var nuevo = new CurriculumSeleccion
                    {
                        CurriculumID = curriculumId,
                        TipoDatoCurriculumID = tipoDatoCurriculumID,
                        TipoDatoID = realId,
                        //Orden = index++
                    };
                    _context.Add(nuevo);
                }

                await _context.SaveChangesAsync();

                // ⚠ Aquí NO haces commit todavía.
                // Primero intentamos generar el PDF
                var pdfBytes = await generarDocumento(curriculumId);

                // Si todo salió bien => commit
                await tx.CommitAsync();

                AlmacenarCv(curriculumId, idUsuario, pdfBytes);

                var nombrePdf = await _context.Curriculum.Where(c => c.Id == curriculumId).Select(c => c.Nombre).FirstOrDefaultAsync();

                return File(pdfBytes, "application/pdf", nombrePdf + ".pdf");
            }
            catch (Exception ex)
            {
                if (tx?.GetDbTransaction()?.Connection != null)
                {
                    await tx.RollbackAsync();
                }

                return BadRequest("Error generando el PDF: " + ex.Message);
            }
        }

        //Clase DTO que permite acomodar los datos para su registro
        public class ItemSeleccion
        {
            public string Tipo { get; set; }
            public string Id { get; set; }
        }

        //Método que configura y genera el Curriculum en PDF
        public async Task<byte[]> generarDocumento(int curriculumId)
        {
            var seleccion = await _context.CurriculumSeleccion
                        .Where(s => s.CurriculumID == curriculumId)
                        .ToListAsync();

            var datos = new List<DatosBasicos>();
            var habilidades = new List<Habilidad>();
            var laborales = new List<ExperienciaLaboral>();
            var academicos = new List<FormacionAcademica>();
            var conocimientos = new List<Conocimiento>();
            var enlaces = new List<Link>();

            foreach (var s in seleccion)
            {
                switch (s.TipoDatoCurriculumID)
                {
                    //case "Perfil":
                    case 1:
                        var d = await _context.DatosBasicos.FindAsync(s.TipoDatoID);
                        if (d != null) datos.Add(d);
                        break;
                    case 2:
                        var h = await _context.Habilidades.FindAsync(s.TipoDatoID);
                        if (h != null) habilidades.Add(h);
                        break;
                    case 3:
                        var c = await _context.Conocimientos.FindAsync(s.TipoDatoID);
                        if (c != null) conocimientos.Add(c);
                        break;
                    case 4:
                        var l = await _context.ExperienciaLaboral.FindAsync(s.TipoDatoID);
                        if (l != null) laborales.Add(l);
                        break;
                    case 5:
                        var a = await _context.FormacionAcademica.FindAsync(s.TipoDatoID);
                        if (a != null) academicos.Add(a);
                        break;
                    case 6:
                        var e = await _context.Enlaces.FindAsync(s.TipoDatoID);
                        if (e != null) enlaces.Add(e);
                        break;
                }
            }

            int idUsuario = await _context.Curriculum.Where(c => c.Id == curriculumId).Select(c => c.UsuarioID).FirstOrDefaultAsync();

            var encabezado = await _context.Encabezados.Where(e => e.UsuarioID == idUsuario).Select(e => e.ValorEncabezado).FirstOrDefaultAsync();

            var nombre = encabezado;
            int paddingBottomSecciones = 20;
            int paddingBottomTitulos = 13;

            // Columnas consistentes en todas las secciones
            const float ColLeft = 1;
            const float ColRight = 1;

            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(80);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // HEADER
                    page.Header()
                        .PaddingBottom(20) // OK: Padding sobre el contenedor
                        .Text(text =>
                        {
                            text.AlignCenter();
                            text.Line(nombre).Bold().Underline().FontSize(24);
                        });

                    // CONTENIDO
                    page.Content().Column(col =>
                    {
                        //
                        // DATOS PERSONALES
                        //
                        if (datos.Any())
                        {
                            // aplica padding al item (contenedor) ANTES de Text(...)
                            if (datos.Any())
                            {
                                // aplica padding al item (contenedor) ANTES de Text(...)
                                col.Item().PaddingBottom(paddingBottomTitulos).Text("Datos Personales").SemiBold().FontSize(18);

                                foreach (var d in datos)
                                {
                                    col.Item().Row(r =>
                                    {
                                        r.RelativeColumn(ColLeft).Text($"{d.NombreDato}:").SemiBold().FontSize(12);
                                        r.RelativeColumn(ColRight).Text(text =>
                                        {
                                            ApplyLinkStyling(text, d.ValorDato ?? "");
                                            // El FontSize se aplica DENTRO del bloque Text
                                            // El estilo base se hereda del FontSize(10) configurado en page.DefaultTextStyle()
                                            // Si necesitas tamaño específico aquí, usa: text.FontSize(12);
                                        });
                                        // Quita el .FontSize(12) de aquí ↓
                                    });
                                }

                                // Padding para separar secciones (aplicado a un item vacío o al siguiente item)
                                col.Item().PaddingBottom(paddingBottomSecciones).Element(x => { });
                            }
                        }


                        //
                        // HABILIDADES
                        //
                        if (habilidades.Any())
                        {
                            //col.Item().PaddingBottom(5).Text("Habilidades").SemiBold().FontSize(18);

                            col.Item().Row(r =>
                            {
                                r.RelativeColumn(ColLeft).Text("Habilidades:").SemiBold().FontSize(18);

                                r.RelativeColumn(ColRight)
                                    .Text(string.Join(" – ", habilidades.Select(h => h.Descripcion)))
                                    .FontSize(12)
                                    .WrapAnywhere();
                            });

                            col.Item().PaddingBottom(paddingBottomSecciones).Element(x => { });
                        }

                        //
                        // CONOCIMIENTOS
                        //
                        if (conocimientos.Any())
                        {
                            //col.Item().PaddingBottom(5).Text("Habilidades").SemiBold().FontSize(18);

                            col.Item().Row(r =>
                            {
                                r.RelativeColumn(ColLeft).Text("Conocimientos:").SemiBold().FontSize(18);

                                r.RelativeColumn(ColRight)
                                    .Text(string.Join(" – ", conocimientos.Select(h => h.Descripcion)))
                                    .FontSize(12)
                                    .WrapAnywhere();
                            });

                            col.Item().PaddingBottom(paddingBottomSecciones).Element(x => { });
                        }

                        //
                        // ANTECEDENTES ACADÉMICOS
                        //
                        if (academicos.Any())
                        {
                            col.Item().PaddingBottom(paddingBottomTitulos).Text("Antecedentes Académicos").SemiBold().FontSize(18);

                            foreach (var a in academicos)
                            {
                                var tipo = _context.TipoInstitucion.FirstOrDefault(t => t.ID == a.TipoInstitucionID)?.Tipo ?? "";

                                col.Item().Row(r =>
                                {
                                    string termino = a.AnhoTermino?.ToString() ?? "Presente";

                                    r.RelativeColumn(ColLeft)
                                        .Text($"({a.AnhoInicio} - {termino})").FontSize(12);

                                    r.RelativeColumn(ColRight).Column(c =>
                                    {
                                        if(a.Descripcion == " ")
                                        {
                                            c.Item().Text($"{a.Carrera} - {tipo} {a.NombreInstitucion} {a.Ciudad}. {a.Descripcion}").SemiBold().FontSize(12);
                                        }
                                        else
                                        {
                                            c.Item().Text(text =>
                                            {
                                                // Texto normal
                                                text.Span($"{a.Carrera} - {tipo} {a.NombreInstitucion} {a.Ciudad} - ")
                                                    .SemiBold()
                                                    .FontSize(12);

                                                // Texto con Markdown
                                                RenderMarkdown(text, a.Descripcion ?? "", 12);
                                            });
                                        }
                                        //c.Item().Text(a.NombreInstitucion).FontSize(12);
                                    });
                                });

                                col.Item().PaddingBottom(paddingBottomSecciones).Element(x => { });
                            }
                        }


                        //
                        // ANTECEDENTES LABORALES
                        //
                        if (laborales.Any())
                        {
                            col.Item().PaddingBottom(paddingBottomTitulos).Text("Antecedentes Laborales").SemiBold().FontSize(18);

                            foreach (var l in laborales)
                            {
                                col.Item().Row(r =>
                                {
                                    string fin = l.FechaTermino?.ToString("dd/MM/yyyy") ?? "Presente";
                                    string inicio = l.FechaInicio.ToString("dd/MM/yyyy");

                                    r.RelativeColumn(ColLeft).Text($"({inicio} - {fin})").FontSize(12);

                                    r.RelativeColumn(ColRight).Column(c =>
                                    {
                                        c.Item().Text(l.Empresa).SemiBold().FontSize(12);
                                        c.Item().Text(l.Descripcion ?? "").FontSize(12);
                                    });
                                });

                                col.Item().PaddingBottom(paddingBottomSecciones).Element(x => { });
                            }
                        }
                        //
                        // ENLACES
                        //
                        if (enlaces.Any())
                        {
                            // aplica padding al item (contenedor) ANTES de Text(...)
                            if (enlaces.Any())
                            {
                                // aplica padding al item (contenedor) ANTES de Text(...)
                                col.Item().PaddingBottom(paddingBottomTitulos).Text("Enlaces").SemiBold().FontSize(18);

                                foreach (var r in enlaces)
                                {
                                    col.Item().Row(e =>
                                    {
                                        e.RelativeColumn(ColLeft).Text($"{r.Titulo}:").SemiBold().FontSize(12);
                                        e.RelativeColumn(ColRight).Text(text =>
                                        {
                                            ApplyLinkStyling(text, r.Enlace ?? "");
                                            // El FontSize se aplica DENTRO del bloque Text
                                            // El estilo base se hereda del FontSize(10) configurado en page.DefaultTextStyle()
                                            // Si necesitas tamaño específico aquí, usa: text.FontSize(12);
                                        });
                                        // Quita el .FontSize(12) de aquí ↓
                                    });
                                }

                                // Padding para separar secciones (aplicado a un item vacío o al siguiente item)
                                col.Item().PaddingBottom(paddingBottomSecciones).Element(x => { });
                            }
                        }
                    });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Row(row =>
                        {
                            row.AutoItem().Text(text =>
                            {
                                text.Span("Generado: ").SemiBold();
                                text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                                text.Span(" - Hecho con: ");
                            });

                            row.AutoItem()
                                .Hyperlink("https://github.com/jorgear90/Proyecto_CV")
                                .Text(" Proyecto CV")
                                .Underline()
                                .FontColor(Colors.Blue.Medium);
                        });

                });
            })


            .GeneratePdf(); // genera byte[]

            return bytes;
        }

        //Configura palabras escritas el markdown
        void RenderMarkdown(TextDescriptor text, string markdown, float fontSize = 12)
        {
            var parts = Regex.Split(markdown, @"(\*\*.*?\*\*)");

            foreach (var part in parts)
            {
                if (part.StartsWith("**") && part.EndsWith("**"))
                    text.Span(part.Trim('*')).Bold().FontSize(fontSize);
                else
                    text.Span(part).FontSize(fontSize);
            }
        }


        // helper style
        IContainer CellStyle(IContainer container)
        {
            return container.PaddingVertical(2).PaddingRight(4);
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

        //Método que almacena los cvs en el disco de la aplicación
        public async Task AlmacenarCv(int curriculumId, int idUsuario, byte[] pdfBytes)
        {
            // Ruta base: wwwroot
            var webRoot = _env.WebRootPath;

            // wwwroot/cv-usuarios/{idUsuario}
            var carpetaUsuario = Path.Combine(webRoot, "cv-usuarios", idUsuario.ToString());

            // Crear carpeta si no existe
            if (!Directory.Exists(carpetaUsuario))
            {
                Directory.CreateDirectory(carpetaUsuario);
            }

            // Nombre del archivo: {curriculumId}.pdf
            var rutaArchivo = Path.Combine(carpetaUsuario, $"{curriculumId}.pdf");

            // Guardar el archivo
            await System.IO.File.WriteAllBytesAsync(rutaArchivo, pdfBytes);

        }

        // Método auxiliar para aplicar estilos a enlaces
        private void ApplyLinkStyling(TextDescriptor text, string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                text.Span(input).FontSize(12); // Aplica tamaño aquí
                return;
            }

            var urlPattern = @"(https?://[^\s]+|www\.[^\s]+)";
            var matches = Regex.Matches(input, urlPattern);

            if (matches.Count == 0)
            {
                text.Span(input).FontSize(12); // Aplica tamaño aquí
                return;
            }

            int lastIndex = 0;

            foreach (Match match in matches)
            {
                if (match.Index > lastIndex)
                {
                    text.Span(input.Substring(lastIndex, match.Index - lastIndex))
                        .FontSize(12); // Aplica tamaño aquí
                }

                string url = match.Value;
                string fullUrl = url.StartsWith("http") ? url : $"https://{url}";

                text.Hyperlink(match.Value, match.Value)
                    .FontColor(Colors.Blue.Medium)
                    .Underline()
                    .FontSize(12);

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < input.Length)
            {
                text.Span(input.Substring(lastIndex))
                    .FontSize(12); // Aplica tamaño aquí
            }
        }

        private bool CurriculumExists(int id)
        {
            return _context.Curriculum.Any(e => e.Id == id);
        }
    }
}
