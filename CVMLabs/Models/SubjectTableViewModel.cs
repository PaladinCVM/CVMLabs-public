using CVMLabs.Domain.Entities;

namespace CVMLabs.Models
{
    public class SubjectTableViewModel
    {
        public SubjectModel SubjectModel { get; set; } = new();
        public StudentModel StudentModel { get; set; } = new();
        public LessonModel LessonModel { get; set; } = new();
        public AttendanceStateModel AttendanceStateModel { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();
    }
}
