using System.ComponentModel.DataAnnotations;

namespace CVMLabs.Domain.Entities
{
    public class StudentModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
