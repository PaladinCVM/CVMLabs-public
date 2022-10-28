using CVMLabs.Domain.Entities;
using CVMLabs.Domain.Repositories.Abstract;
using CVMLabs.Service;
using Microsoft.EntityFrameworkCore;

namespace CVMLabs.Domain.Repositories.EntityFramework
{
    public class EFSubjectsRepository : ISubjectsRepository
    {
        private readonly AppDbContext _dbContext;
        public EFSubjectsRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public IEnumerable<SubjectModel> GetAllSubjects() => _dbContext.Subjects;

        public SubjectModel GetSubjectById(int id) => _dbContext.Subjects
           /* .Include(subject => subject.Lessons)*/
            .FirstOrDefault(subject => subject.Id == id);

        public void SaveSubject(SubjectModel subject)
        {
            if (subject.Id == default) _dbContext.Entry(subject).State = EntityState.Added;
            else _dbContext.Entry(subject).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void DeleteSubjectById(int id)
        {
            _dbContext.Subjects?.Remove(GetSubjectById(id));
            _dbContext.SaveChanges();
        }
    }
}
