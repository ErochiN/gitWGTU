using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitВГТУ
{
    [Table("users")]
    public class users
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Group { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Hash_password { get; set; }

    }
}
