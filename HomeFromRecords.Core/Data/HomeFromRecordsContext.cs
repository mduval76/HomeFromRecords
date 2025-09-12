using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HomeFromRecords.Core.Data.Entities;
using System.Reflection.Emit;

namespace HomeFromRecords.Core.Data {
    public class HomeFromRecordsContext(DbContextOptions<HomeFromRecordsContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options) {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<RecordLabel> RecordLabels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<Artist>()
                .HasMany(a => a.RecordLabels)
                .WithMany(r => r.Artists);

            builder.SeedAdmin();
        }
    }
}
