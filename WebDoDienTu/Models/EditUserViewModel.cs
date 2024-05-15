using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebDoDienTu.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
        public bool? IsBlocked { get; set; }
        public string ConcurrencyStamp { get; set; }
        // Thêm các thuộc tính khác nếu cần
    }

}
