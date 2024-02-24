using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HomeFromRecords.Core.Data.Entities;
using System.Reflection.Emit;

namespace HomeFromRecords.Core.Data {
    public class HomeFromRecordsContext : IdentityDbContext<User, IdentityRole<Guid>, Guid> {
        public DbSet<User> Users { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<RecordLabel> RecordLabels { get; set; }

        public HomeFromRecordsContext(DbContextOptions<HomeFromRecordsContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<Artist>()
                .HasMany(a => a.RecordLabels)
                .WithMany(r => r.Artists);

            builder.SeedAdmin();
        }
    }
}
