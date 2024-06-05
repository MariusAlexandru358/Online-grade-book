using CatalogOnline.ContextModels;
using CatalogOnline.Logic;
using CatalogOnline.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CatalogOnline.Controllers
{
    public class ProfesorController : Controller
    {
        private readonly CatalogOnlineContext _catalogOnlineContext;

        public ProfesorController(CatalogOnlineContext catalogOnlineContext)
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

            var user = _catalogOnlineContext.Profesor.FirstOrDefault(p => p.Email.ToLower() == loggedInEmail.ToLower());
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
            var loggedInProfEmail = User.FindFirstValue(ClaimTypes.Email);
            var profesor = _catalogOnlineContext.Profesor.FirstOrDefault(p => p.Email.ToLower() == loggedInProfEmail.ToLower());

            var query = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Profesor)
                .Include(g => g.Student)
                .Include(g => g.Materie)
                .Where(g => g.Profesor.Email.ToLower() == loggedInProfEmail.ToLower()).ToList();

            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            /*List<List<ProfesorMaterieStudent>> grupPredare = new List<List<ProfesorMaterieStudent>>();
            for (int i = 1; i <= 4; i++)
            {
                grupPredare.Add(query.FindAll(g =>
                        g.Student.AnInscriere + g.An - (currentMonth > 8 ? 1 : 0) == currentYear
                        && g.An == i));
            }
            List<List<ProfesorMaterieStudent>> groupedGrupPredare = new List<List<ProfesorMaterieStudent>>();
            foreach (var list in grupPredare)
            {
                var groupedLists = list.GroupBy(g => g.MaterieId)
                                       .Select(group => group.ToList())
                                       .ToList();

                groupedGrupPredare.AddRange(groupedLists);
            }
            grupPredare = groupedGrupPredare;
*/

            /*
                        var grupPredare = query
                            .GroupBy(g => new
                            {
                                StudyYear = g.Student.AnInscriere + g.An - (currentMonth > 8 ? 1 : 0),
                                g.An
                                //g.Semestru
                            })
                            .Where(g => g.Key.StudyYear == currentYear)
                            .SelectMany(g => g
                                .GroupBy(innerGroup => innerGroup.MaterieId)
                                .Select(innerGroup => innerGroup.ToList())
                                )
                            .OrderBy(list => list.First().An)
                            .ThenBy(list => list.First().Semestru)
                            .ToList();
            */





            /*var grupPredare = query
        .GroupBy(g => new
        {
            StudyYear = g.Student.AnInscriere + g.An - (currentMonth > 8 ? 1 : 0),
            g.An,
            g.Semestru
        })
        .Where(g => g.Key.StudyYear == currentYear)
        .Select(g => new NoteListDTO
        {
            //An = g.Key.An,
            //Semestru = g.Key.Semestru,
            NoteList = g.Select(s => new NoteDTO
            {
                Id = s.Id,
                ProfesorId = s.ProfesorId,
                MaterieId = s.MaterieId,
                StudentId = s.StudentId,
                Materie = s.Materie,
                Student = s.Student,
                An = s.An,
                Semestru = s.Semestru,
                Nota = s.Nota
            }).ToList()
        })
        .OrderBy(g => g.NoteList[0].An)
        .ThenBy(g => g.NoteList[0].Semestru)
        .ToList();*/






            /*var grupRestante = query.FindAll(g => g.Nota < 5);
            */



            /*
            var grupRestante = query
    .Where(g => g.Nota < 5)
    .Select(s => new NoteDTO
    {
        Id = s.Id,
        ProfesorId = s.ProfesorId,
        MaterieId = s.MaterieId,
        StudentId = s.StudentId,
        Materie = s.Materie,
        Student = s.Student,
        An = s.An,
        Semestru = s.Semestru,
        Nota = s.Nota
    })
    .ToList();
            grupPredare.Add(new NoteListDTO { NoteList = grupRestante });
*/


            /*var grupPredare = query
    .GroupBy(g => new
    {
        StudyYear = g.Student.AnInscriere + g.An - (currentMonth > 8 ? 1 : 0),
        g.An,
        g.Semestru,
        g.MaterieId
    })
    .Where(g => g.Key.StudyYear == currentYear)
    .Select(g => g.Select(innerGroup => new NoteDTO
    {
        Id = innerGroup.Id,
        ProfesorId = innerGroup.ProfesorId,
        MaterieId = innerGroup.MaterieId,
        StudentId = innerGroup.StudentId,
        Materie = innerGroup.Materie, // Assuming Materie is an object with a Nume property
        Student = innerGroup.Student, // Assuming Student is an object with a Nume property
        An = innerGroup.An,
        Semestru = innerGroup.Semestru,
        Nota = innerGroup.Nota
    }).ToList())
    .OrderBy(list => list.First().An)
    .ThenBy(list => list.First().Semestru)
    .ToList();


            var grupRestante = query
                .Where(g => g.Nota < 5)
                .Select(s => new NoteDTO
                {
                    Id = s.Id,
                    ProfesorId = s.ProfesorId,
                    MaterieId = s.MaterieId,
                    StudentId = s.StudentId,
                    Materie = s.Materie, // Assuming Materie is an object with a Nume property
                    Student = s.Student, // Assuming Student is an object with a Nume property
                    An = s.An,
                    Semestru = s.Semestru,
                    Nota = s.Nota
                })
                .ToList();

            grupPredare.Add(grupRestante);


            return View("Index", grupPredare);*/




            var grupPredare = query
                .GroupBy(g => new
                {
                    StudyYear = g.Student.AnInscriere + g.An - (currentMonth > 8 ? 1 : 0),
                    g.An,
                    g.Semestru,
                    g.MaterieId
                })
                .Where(g => g.Key.StudyYear == currentYear)
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
                        An = innerGroup.An,
                        Semestru = innerGroup.Semestru,
                        Nota = innerGroup.Nota
                    }).ToList()
                })
                .OrderBy(list => list.NoteList.First().An)
                .ThenBy(list => list.NoteList.First().Semestru)
                .ToList();

            var grupRestante = query
                .Where(g => g.Nota < 5)
                .Select(s => new NoteDTO
                {
                    Id = s.Id,
                    ProfesorId = s.ProfesorId,
                    MaterieId = s.MaterieId,
                    StudentId = s.StudentId,
                    Materie = s.Materie,
                    Student = s.Student,
                    An = s.An,
                    Semestru = s.Semestru,
                    Nota = s.Nota
                })
                .ToList();

            grupPredare.Add(new NoteListDTO { NoteList = grupRestante });

            var unreadNotificationsCount = _catalogOnlineContext.MesajProfesor
                .Count(n => n.ProfesorId == profesor.Id && !n.IsRead);

            ViewBag.UnreadNotificationsCount = unreadNotificationsCount;

            return View("Index", grupPredare);


        }

        public IActionResult AddNota(int id)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            //ViewBag.Categories = _newsContext.Category.Select(cat => new SelectListItem { Value = cat.Id.ToString(), Text = cat.Name }).ToList();
            //var nw = _newsContext.News.Include(nw => nw.Category).FirstOrDefault(nw => nw.Id == id);
            return View("AddNota", null);
        }

        public IActionResult EditNota(List<NoteListDTO> noteList)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            foreach (var list in noteList)
            {
//                var nota = list;
                foreach(var nota in list.NoteList)
                {
                    if (nota.Nota < 1 || nota.Nota > 10)
                    {
                        ModelState.AddModelError("Nota", "Nota trebuie sa fie intre 1 si 10.");
                        return View("Error"); // Return to an error view or handle as appropriate
                    }
                    var existingGrade = _catalogOnlineContext.ProfesorMaterieStudent
                        .Include(g => g.Materie).Include(g => g.Student).FirstOrDefault(g => g.Id == nota.Id);
                    if (existingGrade != null)
                    {
                        if (existingGrade.Nota != nota.Nota)
                        {
                            existingGrade.Nota = nota.Nota;
                            var notification = new NotificareNota
                            {
                                StudentId = existingGrade.StudentId,
                                Message = $"Nota modificata la materia {existingGrade.Materie.Nume}.",
                                CreatedAt = DateTime.Now,
                                IsRead = false
                            };
                            _catalogOnlineContext.NotificareNota.Add(notification);
                        }
                        _catalogOnlineContext.ProfesorMaterieStudent.Update(existingGrade);
                        _catalogOnlineContext.SaveChanges();
                    }
                }
                
            }
            

            // Redirect to the index page or wherever appropriate
            return RedirectToAction("Index");
        }



        public IActionResult Notificare()
        {
            var loggedInEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var profesor = _catalogOnlineContext.Profesor.FirstOrDefault(s => s.Email.ToLower() == loggedInEmail.ToLower());


            var notifications = _catalogOnlineContext.MesajProfesor
                .Where(n => n.ProfesorId == profesor.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            var notificationsDisplay = notifications.Select(n => new MesajProfesor
            {
                Id = n.Id,
                ProfesorId = n.ProfesorId,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            var unreadNotifications = notifications.Where(n => !n.IsRead).ToList();

            foreach (var n in unreadNotifications)
            {
                n.IsRead = true;
                _catalogOnlineContext.MesajProfesor.Update(n);
            }
            _catalogOnlineContext.SaveChanges();
            return View("Notifications", notificationsDisplay);
        }


    }
}
