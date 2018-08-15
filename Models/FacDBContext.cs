using Microsoft.EntityFrameworkCore;

namespace facdb.Models {
    public class FacDBContext : DbContext {
        public FacDBContext(DbContextOptions<FacDBContext> options) : base(options) {

        }

        public DbSet<Room> rooms { get; set; }
    }
}