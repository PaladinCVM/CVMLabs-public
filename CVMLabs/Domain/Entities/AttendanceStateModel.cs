using static CVMLabs.Service.Constants;

namespace CVMLabs.Domain.Entities
{
    public class AttendanceStateModel : EntityModel
    {
        public StudentModel Student { get; set; }
        public LessonState State { get; set; }
        public int LessonId { get; set; }
        public LessonModel Lesson { get; set; }
    }
}
