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

namespace CurriculumVitaeApp.Controllers
{
    public class CurriculumController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IdProtector _idProtector;

        public CurriculumController(AppDbContext context, IdProtector idProtector)
        {
            _context = context;
            _idProtector = idProtector;
        }

        //Método GET para la vista parcial DatosBasicos
        public async Task<IActionResult> SelectorDatosBasicos()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var datosBasicos = _context.DatosBasicos.Include(d => d.Usuarios).Where(d => d.UsuarioID == idUsuario);
            return View(await datosBasicos.ToListAsync());
        }

        //Método GET para la vista parcial SelectorHabilidad
        public async Task<IActionResult> SelectorHabilidad()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var habilidades = _context.Habilidades.Include(d => d.Usuarios).Where(d => d.UsuarioID == idUsuario);

            return View(await habilidades.ToListAsync());
        }

        //Método GET para la vista parcial SelectorConocimiento
        public async Task<IActionResult> SelectorConocimiento()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var conocimientos = _context.Conocimientos.Include(d => d.Usuarios).Where(d => d.UsuarioID == idUsuario);

            return View(await conocimientos.ToListAsync());
        }

        //Método GET para la vista parcial SelectorFormacionAcademica
        public async Task<IActionResult> SelectorFormacionAcademica()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var antecedentesAcademicos = _context.FormacionAcademica.Include(d => d.TipoInstitucion).Where(d => d.UsuarioID == idUsuario);

            ViewData["TipoInstitucionID"] = new SelectList(_context.TipoInstitucion, "ID", "Tipo");

            return View(await antecedentesAcademicos.ToListAsync());
        }

        //Método GET para la vista parcial SelectorExperienciaLaboral
        public async Task<IActionResult> SelectorExperienciaLaboral()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var antecedentesLaborales = _context.ExperienciaLaboral.Include(d => d.Usuarios).Where(d => d.UsuarioID == idUsuario);

            return View(await antecedentesLaborales.ToListAsync());
        }

        // GET: Curriculum/Details/5
        public async Task<IActionResult> VistaSeleccionar()
        {
            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            var encabezado = await _context.Encabezados.Where( e => e.UsuarioID == idUsuario).Select( e => e.ValorEncabezado).FirstOrDefaultAsync();

            ViewBag.Encabezado = encabezado;

            return View();
        }

        //Este método recepciona y entrega los datos seleccionados para generar el curriculum
        [HttpPost]
        public async Task<IActionResult> GenerarPDF(string seleccionadosJson, string valor)
        {
            var seleccionados = JsonConvert.DeserializeObject<List<ItemSeleccion>>(seleccionadosJson ?? "[]");

            var idUsuario = await getIdUsuario();

            if (idUsuario == 0)
                return RedirectToAction("Login", "Usuarios");

            int curriculumId = await _context.Curriculum.Where( c => c.UsuarioID == idUsuario).Select(c => c.Id).FirstOrDefaultAsync();

            int valorId = await _context.Encabezados.Where( e => e.UsuarioID == idUsuario).Select(e => e.Id).FirstOrDefaultAsync();

            if(curriculumId == 0)
            {
                var nuevoCv = new Curriculum
                {
                    UsuarioID = idUsuario,
                    //Orden = index++
                };
                _context.Add(nuevoCv);
                await _context.SaveChangesAsync();

                curriculumId = nuevoCv.Id;
            }

            if (valorId == 0)
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
            else
            {
                var encabezadoExiste = await _context.Encabezados.Where(e => e.UsuarioID == idUsuario).FirstOrDefaultAsync();
                encabezadoExiste.ValorEncabezado = valor;

                _context.Update(encabezadoExiste);
                await _context.SaveChangesAsync();
            }

            int index = 0;

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var antiguos = _context.CurriculumSeleccion.Where(c => c.CurriculumID == curriculumId);
                _context.RemoveRange(antiguos);
                await _context.SaveChangesAsync();

                foreach (var s in seleccionados)
                {
                    var nuevo = new CurriculumSeleccion
                    {
                        CurriculumID = curriculumId,
                        TipoDato = s.Tipo,
                        TipoDatoID = s.Id
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

                return File(pdfBytes, "application/pdf", "Curriculum.pdf");
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
            public int Id { get; set; }
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

            foreach (var s in seleccion)
            {
                switch (s.TipoDato)
                {
                    //case "Perfil":
                    case "DatosBasicos":
                        var d = await _context.DatosBasicos.FindAsync(s.TipoDatoID);
                        if (d != null) datos.Add(d);
                        break;
                    case "Habilidades":
                        var h = await _context.Habilidades.FindAsync(s.TipoDatoID);
                        if (h != null) habilidades.Add(h);
                        break;
                    case "Conocimientos":
                        var c = await _context.Conocimientos.FindAsync(s.TipoDatoID);
                        if (c != null) conocimientos.Add(c);
                        break;
                    case "ExperienciaLaboral":
                        var l = await _context.ExperienciaLaboral.FindAsync(s.TipoDatoID);
                        if (l != null) laborales.Add(l);
                        break;
                    case "FormacionAcademica":
                        var a = await _context.FormacionAcademica.FindAsync(s.TipoDatoID);
                        if (a != null) academicos.Add(a);
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
                                        c.Item().Text($"{a.Carrera} - {tipo} {a.NombreInstitucion} {a.Ciudad}").SemiBold().FontSize(12);
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
                    });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generado: ").SemiBold();
                            x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                        });
                });
            })


            .GeneratePdf(); // genera byte[]

            return bytes;
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
