using System.ComponentModel.DataAnnotations;

namespace CVMLabs.Domain.Entities
{
    public class SubjectModel : EntityModel
    {
        [Required]
        public string? SubjectTitle { get; set; }
    }
}
