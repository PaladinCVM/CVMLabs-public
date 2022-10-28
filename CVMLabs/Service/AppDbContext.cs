using CVMLabs.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CVMLabs.Service
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        public DbSet<StudentModel>? Students { get; set; }
        public DbSet<SubjectModel>? Subjects { get; set; }
        public DbSet<LessonModel>? Lessons { get; set; }
    }
}
