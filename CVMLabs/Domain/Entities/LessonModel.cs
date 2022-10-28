namespace CVMLabs.Domain.Entities
{
    public class LessonModel : EntityModel
    {
        public SubjectModel Subject { get; set; }
        public DateTime LessonDate { get; set; } = DateTime.Now.Date;
        public List<AttendanceStateModel> AttendanceStates { get; set; } = new();
    }
}
