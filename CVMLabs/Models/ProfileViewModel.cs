using CVMLabs.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CVMLabs.Models
{
    public class ProfileViewModel
    {
        public UserInfo UserInfoModel { get; set; } = new();
        public UserPassword UserPasswordModel { get; set; } = new();
        public SubjectModel SubjectModel { get; set; } = new();
        public StudentModel StudentModel { get; set; } = new();
    }

    public class UserInfo
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
    }

    public class UserPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords don't match")]
        public string? ConfirmNewPassword { get; set; }
    }
}