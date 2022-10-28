using CVMLabs.Domain.Entities;

namespace CVMLabs.Domain.Repositories.Abstract
{
    public interface ISubjectsRepository
    {
        IEnumerable<SubjectModel> GetAllSubjects();
        SubjectModel GetSubjectById(int id);
        void SaveSubject(SubjectModel subject);
        void DeleteSubjectById(int id);
    }
}
