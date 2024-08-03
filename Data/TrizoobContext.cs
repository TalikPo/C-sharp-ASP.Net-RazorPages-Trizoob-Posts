using Microsoft.EntityFrameworkCore;

namespace Trizoob.Data
{
    public class TrizoobContext : DbContext
    {
        public TrizoobContext (DbContextOptions<TrizoobContext> options) : base(options){}

        public DbSet<Trizoob.Models.Author> Author { get; set; } = default!;
        public DbSet<Trizoob.Models.Publication> Publication { get; set; } = default!;
        public DbSet<Trizoob.Models.Like> Likes { get; set; } = default!;
        public DbSet<Trizoob.Models.Dislike> Dislikes { get; set; } = default!;
    }
}
