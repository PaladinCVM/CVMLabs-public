using CVMLabs.Domain.Entities;
using CVMLabs.Domain.Repositories.Abstract;
using CVMLabs.Service;
using Microsoft.EntityFrameworkCore;

namespace CVMLabs.Domain.Repositories.EntityFramework
{
    public class EFLessonsRepository : ILessonsRepository
    {
        private readonly AppDbContext _dbContext;
        public EFLessonsRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public IQueryable<LessonModel>? GetAllLessons() => _dbContext.Lessons?.Include(e => e.Subject);

        public LessonModel? GetLessonById(int id) => _dbContext.Lessons?
            .Include(e => e.Subject)
            .Include(e => e.AttendanceStates)
            .ThenInclude(e => e.Student)
            .FirstOrDefault(lesson => lesson.Id == id);

        public void SaveLesson(LessonModel lesson)
        {
            if (lesson.Id == default)
            {
                _dbContext.Entry(lesson).State = EntityState.Added;
                lesson.AttendanceStates.ForEach(state => _dbContext.Entry(state).State = EntityState.Added);
            }
            else
            {
                _dbContext.Entry(lesson).State = EntityState.Modified;

                if (lesson.AttendanceStates.Exists(e => e.Lesson.Id == 0))
                {
                    var newState = lesson.AttendanceStates.First(e => e.Lesson.Id == 0);
                    _dbContext.Entry(newState).State = EntityState.Added;
                }
            }
            _dbContext.SaveChanges();
        }

        public void DeleteLessonById(int id)
        {
            _dbContext.Lessons?.Remove(GetLessonById(id)!);
            _dbContext.SaveChanges();
        }
    }
}
