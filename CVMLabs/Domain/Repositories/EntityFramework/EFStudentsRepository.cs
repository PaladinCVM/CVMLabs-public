using CVMLabs.Domain.Entities;
using CVMLabs.Domain.Repositories.Abstract;
using CVMLabs.Service;
using Microsoft.EntityFrameworkCore;

namespace CVMLabs.Domain.Repositories.EntityFramework
{
    public class EFStudentsRepository : IStudentsRepository
    {
        private readonly AppDbContext _dbContext;
        public EFStudentsRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public IEnumerable<StudentModel> GetAllStudents() => _dbContext.Students;

        public StudentModel GetStudentById(int id) => _dbContext.Students.FirstOrDefault(subject => subject.Id == id);

        public void SaveStudent(StudentModel student)
        {
            if (student.Id == default) _dbContext.Entry(student).State = EntityState.Added;
            else _dbContext.Entry(student).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void DeleteStudentById(int id)
        {
            _dbContext.Students?.Remove(GetStudentById(id));
            _dbContext.SaveChanges();
        }
    }
}
