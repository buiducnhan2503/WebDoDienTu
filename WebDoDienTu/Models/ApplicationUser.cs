using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebDoDienTu.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [DisplayName("Họ")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Tên")]
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
    }
}
