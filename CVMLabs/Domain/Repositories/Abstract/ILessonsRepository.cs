using CVMLabs.Domain.Entities;

namespace CVMLabs.Domain.Repositories.Abstract
{
    public interface ILessonsRepository
    {
        IQueryable<LessonModel>? GetAllLessons();
        LessonModel? GetLessonById(int id);
        void SaveLesson(LessonModel lesson);
        void DeleteLessonById(int id);
    }
}
