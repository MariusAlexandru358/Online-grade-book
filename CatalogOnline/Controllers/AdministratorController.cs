using CatalogOnline.ContextModels;
using CatalogOnline.Logic;
using CatalogOnline.Models;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CatalogOnline.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly CatalogOnlineContext _catalogOnlineContext;
        public AdministratorController(CatalogOnlineContext context)
        {
            _catalogOnlineContext = context;
        }

        bool CredentialsIsValid()
        {
            var loggedInEmail = User.FindFirstValue(ClaimTypes.Email);
            if (loggedInEmail == null)
            {
                return false;
            }

            var user = _catalogOnlineContext.Adminstrator.FirstOrDefault(p => p.Email.ToLower() == loggedInEmail.ToLower());
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

            var materii = _catalogOnlineContext.Materie.ToList();
            return View("Index",materii);
        }

        public IActionResult IndexMaterie()
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var materii = _catalogOnlineContext.Materie.ToList();
            return View("IndexMaterie", materii);
        }


        // GET: Administrator/Create
        public IActionResult Create()
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            return View();
        }

        // POST: Administrator/Create
        [HttpPost]
        public IActionResult Create(Materie materie)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            if (ModelState.IsValid)
            {
                _catalogOnlineContext.Add(materie);
                _catalogOnlineContext.SaveChanges();
                return RedirectToAction("IndexMaterie");
            }
            return View(materie);
        }

        // GET: Administrator/Edit/5
        public IActionResult Edit(int? id)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var materie = _catalogOnlineContext.Materie.Find(id);
            if (materie == null)
            {
                return NotFound();
            }
            return View(materie);
        }

        // POST: Administrator/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Materie materie)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            if (id != materie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _catalogOnlineContext.Update(materie);
                    _catalogOnlineContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterieExists(materie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("IndexMaterie");
            }
            return View(materie);
        }

        // GET: Administrator/Delete/5
        public IActionResult Delete(int? id)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var materie = _catalogOnlineContext.Materie
                .FirstOrDefault(m => m.Id == id);
            if (materie == null)
            {
                return NotFound();
            }

            return View(materie);
        }

        // POST: Administrator/Delete/5
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var materie = _catalogOnlineContext.Materie.Find(id);
            _catalogOnlineContext.Materie.Remove(materie);
            _catalogOnlineContext.SaveChanges();
            return RedirectToAction("IndexMaterie");
        }

        private bool MaterieExists(int id)
        {
            return _catalogOnlineContext.Materie.Any(e => e.Id == id);
        }





        public IActionResult IndexCurs()
        {
            if (!CredentialsIsValid())
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

            /*if (year1 == null)
            {
                if (currentMonth > 8)
                {
                    year1 = currentYear;
                    year2 = currentYear + 1;
                }
                else
                {
                    year1 = currentYear - 1;
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
            }*/
            var grupPredare = query
                .GroupBy(g => new
                {
                    StudyYear = g.Student.AnInscriere + g.An - (currentMonth > 8 ? 1 : 0),
                    g.An,
                    g.Semestru,
                    g.MaterieId,
                    g.ProfesorId
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
                        Profesor = innerGroup.Profesor,
                        An = innerGroup.An,
                        Semestru = innerGroup.Semestru,
                        Nota = innerGroup.Nota
                    }).ToList()
                })
                .OrderBy(list => list.NoteList.First().An)
                .ThenBy(list => list.NoteList.First().Semestru)
                .ToList();

            return View("IndexCurs", grupPredare);
        }



        // GET: Administrator/AddStudentToCourse
        public IActionResult AddStudent(int catalogId)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var reff = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Materie)
                .Include(g => g.Profesor)
                .FirstOrDefault(g => g.Id == catalogId);
            var reffList = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Student)
                .Where(g => g.An == reff.An && g.MaterieId == reff.MaterieId )
                .ToList();
            var studentList = _catalogOnlineContext.Student.OrderBy(g => g.AnInscriere).ToList();
            var model = new AddRemoveStudentViewModel
            {
                An = reff.An,
                Semestru = reff.Semestru,
                MaterieId = reff.MaterieId,
                ProfesorId = reff.ProfesorId,
                Materie = reff.Materie,
                Profesor = reff.Profesor,
                StudentList = studentList
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddStudent(int an, int sem, int materieId, int profesorId, int[] selectedStudents)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var materie = _catalogOnlineContext.Materie.FirstOrDefault(m => m.Id == materieId);
            foreach (var studentId in selectedStudents)
            {
                var existingEntry = _catalogOnlineContext.ProfesorMaterieStudent
                    .Include(g => g.Materie)
                    .FirstOrDefault(g => g.An == an && g.MaterieId == materieId && g.ProfesorId == profesorId && g.StudentId == studentId);

                if (existingEntry == null)
                {
                    _catalogOnlineContext.ProfesorMaterieStudent.Add(new ProfesorMaterieStudent
                    {
                        An = an,
                        Semestru = sem,
                        MaterieId = materieId,
                        ProfesorId = profesorId,
                        StudentId = studentId
                    });

                    var notification = new NotificareNota
                    {
                        StudentId = studentId,
                        Message = $"Ai fost adăugat/adăugată la materia {materie.Nume}.",
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    };
                    _catalogOnlineContext.NotificareNota.Add(notification);
                }
            }

            _catalogOnlineContext.SaveChanges();

            return RedirectToAction("IndexCurs");
        }

        // GET: Administrator/RemoveStudentFromCourse
        public IActionResult RemoveStudent(int catalogId)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            var reff = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Materie)
                .Include(g => g.Profesor)
                .FirstOrDefault(g => g.Id == catalogId);
            var reffList = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Student)
                .Where(g => g.An == reff.An && g.MaterieId == reff.MaterieId)
                .ToList();
            var studentList = reffList.Select(g => g.Student).ToList();
            var model = new AddRemoveStudentViewModel
            {
                An = reff.An,
                Semestru = reff.Semestru,
                MaterieId = reff.MaterieId,
                ProfesorId = reff.ProfesorId,
                Materie = reff.Materie,
                Profesor = reff.Profesor,
                StudentList = studentList
            };

            return View(model);
        }

        // POST: Administrator/RemoveStudentFromCourse
        [HttpPost]
        public IActionResult RemoveStudent(int an, int sem, int materieId, int profesorId, int[] selectedStudents)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            foreach (var studentId in selectedStudents)
            {
                var existingEntry = _catalogOnlineContext.ProfesorMaterieStudent
                    .FirstOrDefault(g => g.An == an && g.MaterieId == materieId && g.ProfesorId == profesorId && g.StudentId == studentId);

                if (existingEntry != null)
                {
                    _catalogOnlineContext.ProfesorMaterieStudent.Remove(existingEntry);
                }
            }

            _catalogOnlineContext.SaveChanges();

            return RedirectToAction("IndexCurs");
        }


        public IActionResult ConstructCurs()
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }
            ViewBag.Materie = _catalogOnlineContext.Materie.ToList();
            ViewBag.Profesor = _catalogOnlineContext.Profesor.ToList();

            /*var reff = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Materie)
                .Include(g => g.Profesor)
                .FirstOrDefault(g => g.Id == catalogId);
            var reffList = _catalogOnlineContext.ProfesorMaterieStudent
                .Include(g => g.Student)
                .Where(g => g.An == reff.An && g.MaterieId == reff.MaterieId)
                .ToList();*/
            var studentList = _catalogOnlineContext.Student.OrderBy(g => g.AnInscriere).ToList();
            /*var model = new AddRemoveStudentViewModel
            {
                An = reff.An,
                Semestru = reff.Semestru,
                MaterieId = reff.MaterieId,
                ProfesorId = reff.ProfesorId,
                Materie = reff.Materie,
                Profesor = reff.Profesor,
                StudentList = studentList
            };*/

            return View(studentList);
        }

        [HttpPost]
        public IActionResult ConstructCurs(int an, int sem, string materieName, string profesorName, int[] selectedStudents)
        {
            if (!CredentialsIsValid())
            {
                return RedirectToAction("Privacy", "Home");
            }


            try
            {
                var materie = _catalogOnlineContext.Materie.FirstOrDefault(m => m.Nume == materieName);
                var profesor = _catalogOnlineContext.Profesor.FirstOrDefault(p => p.Nume == profesorName);

                if (an > 4 || an < 1 || sem > 2 || sem < 1)
                {
                    throw new Exception("Anul sau semestrul nepotrivite.");
                }

                foreach (var studentId in selectedStudents)
                {

                    var existingEntry = _catalogOnlineContext.ProfesorMaterieStudent
                        .Include(g => g.Materie)
                        .FirstOrDefault(g => g.An == an && g.MaterieId == materie.Id && g.ProfesorId == profesor.Id && g.StudentId == studentId);

                    if (existingEntry == null)
                    {
                        _catalogOnlineContext.ProfesorMaterieStudent.Add(new ProfesorMaterieStudent
                        {
                            An = an,
                            Semestru = sem,
                            MaterieId = materie.Id,
                            ProfesorId = profesor.Id,
                            StudentId = studentId
                        });

                        var notification = new NotificareNota
                        {
                            StudentId = studentId,
                            Message = $"Ai fost adăugat/adăugată la materia {materie.Nume}.",
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };
                        _catalogOnlineContext.NotificareNota.Add(notification);
                    }
                }

                _catalogOnlineContext.SaveChanges();

                return RedirectToAction("IndexCurs");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Eroare: câmpuri completate greșit");
            }

            ViewBag.Materie = _catalogOnlineContext.Materie.ToList();
            ViewBag.Profesor = _catalogOnlineContext.Profesor.ToList();
            var studentList = _catalogOnlineContext.Student.OrderBy(g => g.AnInscriere).ToList();
            return View(studentList);
        }

    }
}
