using CatalogOnline.ContextModels;
using CatalogOnline.Logic;
using CatalogOnline.Models;
using iText.Commons.Actions.Contexts;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CatalogOnline.Controllers
{
    public class SecretarController : Controller
    {
        private readonly CatalogOnlineContext _catalogOnlineContext;

        public SecretarController(CatalogOnlineContext catalogOnlineContext)
        {
            _catalogOnlineContext = catalogOnlineContext;
        }
        bool CredentialsIsValid()
        {
            var loggedInEmail = User.FindFirstValue(ClaimTypes.Email);
            if (loggedInEmail == null)
            {
                return false;
            }

            var user = _catalogOnlineContext.Secretar.FirstOrDefault(p => p.Email.ToLower() == loggedInEmail.ToLower());
            if (user == default || user == null)
            {
                return false;
            }
            return true;
        }

        public IActionResult Index()
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }

            var years = new List<int>();
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            if (currentMonth > 8)
            {
                currentYear += 1; 
            }

            for (int i = 0; i < 5; i++)
            {
                years.Add(currentYear - i);
            }

            ViewBag.Materie = _catalogOnlineContext.Materie.ToList();
            ViewBag.Profesor = _catalogOnlineContext.Profesor.ToList();

            return View(years);
        }

        [HttpPost]
        public IActionResult SelectYearAndSubject(int selectedYear, string subjectSearch, string profesorSearch)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            int? subjectId = null;
            if (!string.IsNullOrEmpty(subjectSearch))
            {
                var subject = _catalogOnlineContext.Materie
                    .FirstOrDefault(m => m.Nume.ToLower().Contains(subjectSearch.ToLower()));

                if (subject != null)
                {
                    subjectId = subject.Id;
                }
            }

            int? profesorId = null;
            if (!string.IsNullOrEmpty(profesorSearch))
            {
                var professor = _catalogOnlineContext.Profesor
                    .FirstOrDefault(p => p.Nume.ToLower().Contains(profesorSearch.ToLower()));

                if (professor != null)
                {
                    profesorId = professor.Id;
                }
            }

            return RedirectToAction("IndexAn", new { year1 = selectedYear, year2 = selectedYear + 1, subjectId, profesorId });
        }

        public IActionResult IndexAn(int? year1, int? year2, int? subjectId, int? profesorId)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var loggedInEmail = User.FindFirstValue(ClaimTypes.Email);
            if (loggedInEmail == null)
            {
                return RedirectToAction("Privacy", "Home");
            }

            var secretar = _catalogOnlineContext.Secretar.FirstOrDefault(p => p.Email.ToLower() == loggedInEmail.ToLower());
            if (secretar == default || secretar == null)
            {
                return RedirectToAction("Privacy", "Home");
            }

            var query = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Profesor)
                .Include(g => g.Student)
                .Include(g => g.Materie)
                .ToList();

            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            if (year1 == null) { 
                if (currentMonth > 8)
                {
                    year1 = currentYear;
                    year2 = currentYear + 1;
                }
                else
                {
                    year1 = currentYear -1;
                    year2 = currentYear;
                }
            }


            if (subjectId != null)
            {
                query = query.Where(g => g.MaterieId == subjectId.Value).ToList();
            }
            if (profesorId != null)
            {
                query = query.Where(g => g.MaterieId == subjectId.Value).ToList();
            }
            if (year1 != 0)
            {
                query = query.Where(g => g.Student.AnInscriere + g.An - (DateTime.Now.Month > 8 ? 1 : 0) == year1).ToList();
            }
            var grupPredare = query
                .GroupBy(g => new
                {
                    StudyYear = g.Student.AnInscriere + g.An - (currentMonth > 8 ? 1 : 0),
                    g.An,
                    g.Semestru,
                    g.MaterieId,
                    g.ProfesorId
                })
                .Select(g => new NoteListDTO
                {
                    NoteList = g.Select(innerGroup => new NoteDTO
                    {
                        Id = innerGroup.Id,
                        ProfesorId = innerGroup.ProfesorId,
                        MaterieId = innerGroup.MaterieId,
                        StudentId = innerGroup.StudentId,
                        Materie = innerGroup.Materie,
                        Student = innerGroup.Student,
                        Profesor = innerGroup.Profesor,
                        An = innerGroup.An,
                        Semestru = innerGroup.Semestru,
                        Nota = innerGroup.Nota
                    }).ToList()
                })
                .OrderBy(list => list.NoteList.First().An)
                .ThenBy(list => list.NoteList.First().Semestru)
                .ToList();

            
            return View("IndexAn", grupPredare);
        }


        [HttpPost]
        public async Task<IActionResult> ExportCatalogToPdf(int catalogId)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var model = await _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Profesor)
                .Include(g => g.Student)
                .Include(g => g.Materie)
                .Where(g => g.Id == catalogId)
                .ToListAsync();

            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var schoolYear = model.First().Student.AnInscriere + model.First().An - (currentMonth > 8 ? 1 : 0);

            var catalog = await _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Profesor)
                .Include(g => g.Student)
                .Include(g => g.Materie)
                .Where (g => g.MaterieId == model.First().MaterieId 
                            && g.ProfesorId == model.First().ProfesorId
                            && g.Student.AnInscriere + g.An - (currentMonth > 8 ? 1 : 0) == schoolYear
                       )
                .ToListAsync();

            if (catalog == null || !catalog.Any())
            {
                return NotFound("Eroare baza de data.");
            }

            var pdf = GenerateCatalogPdf(catalog);

            return File(pdf, "application/pdf", $"Catalog_{catalog.First().Profesor.Nume}_{catalog.First().Materie.Nume}.pdf");
        }

        private byte[] GenerateCatalogPdf(List<ProfesorMaterieStudent> catalog)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);
                var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "calibri.ttf");
                var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

                var firstEntry = catalog.First();

                var currentYear = DateTime.Now.Year;
                var currentMonth = DateTime.Now.Month;
                var schoolYear = firstEntry.Student.AnInscriere + firstEntry.An - (currentMonth > 8 ? 1 : 0);

                document.Add(new Paragraph($"Anul universitar {schoolYear}-{schoolYear+1}").SetFont(font).SetFontSize(12));
                document.Add(new Paragraph($"Disciplina: {firstEntry.Materie.Nume}").SetFont(font).SetFontSize(12));
                document.Add(new Paragraph($"Profesor: {firstEntry.Profesor.Nume}").SetFont(font).SetFontSize(12));
                document.Add(new Paragraph($"Anul {firstEntry.An}, Semestrul {firstEntry.Semestru}").SetFont(font).SetFontSize(12));

                document.Add(new Paragraph("\nNote:").SetFont(font).SetFontSize(12).SetBold());

                var table = new Table(2).UseAllAvailableWidth();
                table.AddHeaderCell(new Cell().Add(new Paragraph("Student").SetFont(font).SetBold()));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Nota").SetFont(font).SetBold()));

                foreach (var entry in catalog)
                {
                    table.AddCell(new Cell().Add(new Paragraph(entry.Student.Nume).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(entry.Nota.ToString()).SetFont(font)));
                }

                document.Add(table);
                document.Close();
                return memoryStream.ToArray();
            }
        }


        public IActionResult NotifyProf()
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var professors = _catalogOnlineContext.Profesor.ToList();
            ViewBag.CatedraList = professors.Select(p => p.Catedra).Distinct().ToList();
            ViewBag.ProfesorList = professors;
            return View("NotifyProf", null);
        }


        public IActionResult SendMessage()
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var professors = _catalogOnlineContext.Profesor.ToList();
            var groups = new List<string> { "Matematica", "Programare", "Fizica" };

            var model = new SendMessageViewModel
            {
                CatedraList = groups,
                ProfesorList = professors
            };
            //var professors = _catalogOnlineContext.Profesor.ToList();
            ViewBag.CatedraList = professors.Select(p => p.Catedra).Distinct().ToList();
            ViewBag.ProfesorList = professors;
            return View("NotifyProf",model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            if (ModelState.IsValid)
            {
                // Get all professors from the selected groups
                List<Profesor> professorsInGroups = new List<Profesor>();
                if (model.CatedraList != null && model.CatedraList.Count > 0)
                {
                    professorsInGroups = _catalogOnlineContext.Profesor
                                                .Where(p => model.CatedraList.Contains(p.Catedra))
                                                .ToList();
                }

                // Get the selected professors
                List<Profesor> selectedProfessors = new List<Profesor>();
                if (model.ProfesorIdList != null && model.ProfesorIdList.Count > 0)
                {
                    selectedProfessors = _catalogOnlineContext.Profesor
                                                 .Where(p => model.ProfesorIdList.Contains(p.Id))
                                                 .ToList();
                }

                // Combine the selected professors and professors from the selected groups
                var combinedProfessors = selectedProfessors.Union(professorsInGroups).Distinct().ToList();

                // Send messages to all selected professors
                foreach (var professor in combinedProfessors)
                {
                    var message = new MesajProfesor
                    {
                        Message = model.Mesaj,
                        CreatedAt = DateTime.Now,
                        ProfesorId = professor.Id,
                        IsRead = false,
                        Profesor = professor
                    };

                    _catalogOnlineContext.MesajProfesor.Add(message);
                }

                await _catalogOnlineContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Mesajul a fost trimis cu succes.";
                return RedirectToAction("SendMessage");
            }

            // Reload the groups and professors list in case of validation error
            model.CatedraList = new List<string> { "Matematica", "Programare", "Fizica" };
            model.ProfesorList = _catalogOnlineContext.Profesor.Distinct().ToList();
            return View("NotifyProf",model);
        }

    }
}
