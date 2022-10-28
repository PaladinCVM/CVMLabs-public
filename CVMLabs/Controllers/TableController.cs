using CVMLabs.Domain.Entities;
using CVMLabs.Models;
using CVMLabs.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CVMLabs.Service.Constants;

namespace CVMLabs.Controllers
{
    [Authorize]
    public class TableController : Controller
    {

        private readonly DataManager _dataManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public TableController(DataManager dataManager, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _dataManager = dataManager;
            _userManager = userManager;
            _context = context;
        }

        [Route("table/{id}")]
        [HttpGet]
        public async Task<IActionResult> Index(int id, SubjectTableViewModel model)
        {
            model.SubjectModel = _dataManager.Subjects.GetSubjectById(id);
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(appUser => appUser.UserSubjects)
                .Include(appUser => appUser.UserStudents)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (model.SubjectModel is null) return Content("Subject not found");
            if (user?.UserSubjects is null || !user.UserSubjects.Contains(model.SubjectModel)) return Content("Access denied"); // TEST: check if user doesn't have any content

            model.LessonModel.Subject = model.SubjectModel;
            model.Lessons.AddRange(_dataManager.Lessons
                .GetAllLessons()!
                .Include(lesson => lesson.Subject).Where(e=>e.Subject == model.SubjectModel)
                .Include(lesson => lesson.AttendanceStates));

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewLesson(LessonModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(appUser => appUser.UserStudents)
                .Include(appUser => appUser.UserSubjects)
                .SingleOrDefaultAsync(u => u.Id == userId);

            foreach (var student in user!.UserStudents) // TEST: check if user doesn't have any content
            {
                AttendanceStateModel attendanceState = new() { Student = student, State = LessonState.NotStated };
                model.AttendanceStates.Add(attendanceState);
            }

            LessonModel lesson = model;
            _dataManager.Lessons.SaveLesson(lesson);

            int returnId = model.Subject.Id;
            return Redirect($"/table/{returnId}");
        }

        [HttpGet]
        public IActionResult EditLessonModalCall(int id)
        {
            LessonModel? model = _dataManager.Lessons.GetLessonById(id);
            return PartialView("Partial/EditLessonModalPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLesson(LessonModel model, int id)
        {
            model.Id = id;
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(s => s.UserSubjects).SingleOrDefaultAsync(u => u.Id == userId);
            var lessonToEdit = _dataManager.Lessons.GetLessonById(model.Id);

            if (lessonToEdit is null) return Content("Lesson not found");
            if (lessonToEdit.Subject.Id != model.Subject.Id) return Content("Access denied");

            lessonToEdit.LessonDate = model.LessonDate;

            _dataManager.Lessons.SaveLesson(lessonToEdit);

            int returnId = model.Subject.Id;
            return Redirect($"/table/{returnId}");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLesson(SubjectTableViewModel model, int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(s => s.UserSubjects).SingleOrDefaultAsync(u => u.Id == userId);

            var lessons = _dataManager.Lessons.GetAllLessons();
            var chosenLesson = _dataManager.Lessons.GetLessonById(id);

            if (lessons is null || !lessons.Any(e => e.Id == id)) return Content("Lesson not found");
            if (chosenLesson?.Subject.Id != model.SubjectModel.Id) return Content("Access denied");

            _dataManager.Lessons.DeleteLessonById(id);

            int returnId = model.SubjectModel.Id;
            return Redirect($"/table/{returnId}");
        }

        [HttpPost]
        public async Task<IActionResult> SetLessonState(SubjectTableViewModel model)
        {
            var lesson = _dataManager.Lessons.GetLessonById(model.LessonModel.Id);
            var student = _dataManager.Students.GetStudentById(model.StudentModel.Id);

            if (lesson is null) return Content("Lesson not found");
            if (lesson.Subject.Id != model.SubjectModel.Id) return Content("Access denied");

            try
            {
                lesson.AttendanceStates.Find(s => s.Student?.Id == model.StudentModel.Id).State = model.AttendanceStateModel.State;
            }
            catch
            {
                lesson.AttendanceStates.Add(new() { Student = student, State = model.AttendanceStateModel.State });
            }

            _dataManager.Lessons.SaveLesson(lesson);

            return Redirect($"/table/{model.SubjectModel.Id}");
        }
    }
}
