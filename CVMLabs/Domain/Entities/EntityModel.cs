using System.ComponentModel.DataAnnotations;

namespace CVMLabs.Domain.Entities
{
    public abstract class EntityModel
    {
        protected EntityModel() => DateCreated = DateTime.Now;

        [Required]
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Time)]
        public DateTime DateCreated { get; set; }
    }
}
