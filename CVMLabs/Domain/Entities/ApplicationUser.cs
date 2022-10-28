using Microsoft.AspNetCore.Identity;

namespace CVMLabs.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public List<SubjectModel> UserSubjects { get; set; } = new();
        public List<StudentModel> UserStudents { get; set; } = new();
    }
}
