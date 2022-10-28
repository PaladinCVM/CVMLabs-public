using CVMLabs.Domain.Entities;

namespace CVMLabs.Domain.Repositories.Abstract
{
    public interface IStudentsRepository
    {
        IEnumerable<StudentModel> GetAllStudents();
        StudentModel GetStudentById(int id);
        void SaveStudent(StudentModel student);
        void DeleteStudentById(int id);
    }
}
