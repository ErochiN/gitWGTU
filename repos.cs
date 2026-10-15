using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitВГТУ
{
    public class repos
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public bool Public { get; set; }


        [Required]
        public int User_Id { get; set; }
        [ForeignKey("User_Id")]
        public users users { get; set; }
    
    
    }
}
