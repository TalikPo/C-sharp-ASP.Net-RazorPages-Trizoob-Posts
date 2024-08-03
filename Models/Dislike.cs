using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trizoob.Models
{
    public class Dislike
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("FK_Author_123")]
        public int AuthorId { get; set; }
        [ForeignKey("FK_Publication_123")]
        public int PublicationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
