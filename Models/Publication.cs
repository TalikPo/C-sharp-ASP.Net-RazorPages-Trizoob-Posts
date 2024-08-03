using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trizoob.Models
{
    public class Publication
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Header { get; set; }
        [Required]
        [MaxLength(512)]
        public string Description { get; set; }
        [ForeignKey("FK_Author")]
        public int AuthorId { get; set; }
        [MaxLength(255)]
        public string DataUrl { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IsPublic { get; set; }
        public Author Author { get; set; }
    }
}
