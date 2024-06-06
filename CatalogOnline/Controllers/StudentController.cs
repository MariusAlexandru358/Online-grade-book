using CatalogOnline.ContextModels;
using CatalogOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection.Metadata;
using System.Security.Claims;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Bouncycastleconnector;
using iText.Bouncycastle;
using Microsoft.AspNetCore.Components.Web;
using iText.Kernel.Font;
using iText.IO.Font;

namespace CatalogOnline.Controllers
{
    public class StudentController : Controller
    {
        private readonly CatalogOnlineContext _catalogOnlineContext;

        public StudentController(CatalogOnlineContext catalogOnlineContext)
        {
            _catalogOnlineContext = catalogOnlineContext;
        }

        public double calcMedie(List<ProfesorMaterieStudent> note)
        {
            double avg = 0.0;
            foreach (var nota in note)
            {
                avg += (double)nota.Nota;
            }
            int nr = note.Count();
            avg = avg / nr;
            return avg;
        }
        bool CredentialsIsValid()
        {
            var loggedInEmail = User.FindFirstValue(ClaimTypes.Email);
            if (loggedInEmail == null)
            {
                return false;
            }

            var user = _catalogOnlineContext.Student.FirstOrDefault(p => p.Email.ToLower() == loggedInEmail.ToLower());
            if (user == default || user == null)
            {
                return false;
            }
            return true;
        }
        public IActionResult Index(int? selectedYear)
        {
            var loggedInStudentEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var student = _catalogOnlineContext.Student.FirstOrDefault(p => p.Email.ToLower() == loggedInStudentEmail.ToLower());


            var query = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Student)
                .Include(g => g.Materie)
                .Where(g => g.Student.Email.ToLower() == loggedInStudentEmail.ToLower());
            if (selectedYear.HasValue)
            {
                query = query.Where(g => g.An == selectedYear.Value);
            }

            var note = query.ToList();
            int maxYear = 0;
            if (!note.IsNullOrEmpty())
            {
                maxYear = note.Max(g => g.An);
            }

            var viewModel = new 
            {
                Student = student,
                Note = note,
                //Medie = medie,
                Anul = maxYear
            };

            var unreadNotificationsCount = _catalogOnlineContext.NotificareNota
                .Count(n => n.StudentId == student.Id && !n.IsRead);

            ViewBag.UnreadNotificationsCount = unreadNotificationsCount;

            return View("Index", viewModel);
        }

        public IActionResult Notificare()
        {
            var loggedInStudentEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var student = _catalogOnlineContext.Student.FirstOrDefault(s => s.Email.ToLower() == loggedInStudentEmail.ToLower());
            

            var notifications = _catalogOnlineContext.NotificareNota
                .Where(n => n.StudentId == student.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            var notificationsDisplay = notifications.Select(n => new NotificareNota
            {
                Id = n.Id,
                StudentId = n.StudentId,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            var unreadNotifications = notifications.Where(n => !n.IsRead).ToList();

            foreach (var n in unreadNotifications)
            {
                n.IsRead = true;
                _catalogOnlineContext.NotificareNota.Update(n);
            }
            _catalogOnlineContext.SaveChanges();
            return View("Notifications", notificationsDisplay);
        }


        [HttpPost]
        public async Task<IActionResult> RequestCertificate()
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var studentEmail = User.FindFirstValue(ClaimTypes.Email);
            var student = await _catalogOnlineContext.Student.FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null || !IsEnrolledInCurrentYear(student))
            {
                return BadRequest("A apărut o eroare neprevăzută. Vă rugăm încercați din nou mai târziu.");
            }

            var pdf = GenerateCertificatePdf(student);

            return File(pdf, "application/pdf", $"AdeverințăStudent_{student.Nume}.pdf");
        }

        private bool IsEnrolledInCurrentYear(Student student)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            return (student.AnInscriere + student.AnStudiu - (currentMonth > 9 ? 1 : 0) == currentYear) && student.AnStudiu > 0;
        }

        private byte[] GenerateCertificatePdf(Student student)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);
                var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "calibri.ttf");
                var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                
                document.Add(new Paragraph("Adeverință student")
                .SetFont(font).SetFontSize(20)
                .SetBold()
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                document.Add(new Paragraph($"\t{student.Nume}, CNP: ************* este student la " +
                    $"Facultatea de Matematica și Informatică a Universității din București, " +
                    $"înscris în programul de studiu {student.ProgramStudiu} în anul {student.AnStudiu}.")
                    .SetFont(font).SetFontSize(12)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));

                document.Add(new Paragraph("\n\n\n\n"));
                document.Add(new Paragraph("Facultatea de Matematica și Informatică, Universitatea din București")
                    .SetFont(font).SetFontSize(12)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                document.Add(new Paragraph($"Data: {DateTime.Now.ToString("dd.MM.yyyy")}")
                    .SetFont(font).SetFontSize(12)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));
                document.Add(new Paragraph("ștampilă și semnătură:")
                    .SetFont(font).SetFontSize(12).SetOpacity((float?)0.1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));


                document.Close();
                return memoryStream.ToArray();
            }
        }


    }
}
