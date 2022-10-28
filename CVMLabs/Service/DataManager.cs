using CVMLabs.Domain.Repositories.Abstract;

namespace CVMLabs.Service
{
    public class DataManager
    {
        public ISubjectsRepository Subjects { get; set; }
        public ILessonsRepository Lessons { get; set; }
        public IStudentsRepository Students { get; set; }

        public DataManager(ISubjectsRepository subjects, ILessonsRepository lessons, IStudentsRepository students)
        {
            Subjects = subjects;
            Lessons = lessons;
            Students = students;
        }
    }
}
