using Microsoft.EntityFrameworkCore;
using LegalStrategyAdvisor.Api.Data.Entities;

namespace LegalStrategyAdvisor.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Case> Cases { get; set; }
        public DbSet<LegalStrategy> LegalStrategies { get; set; }
        public DbSet<Precedent> Precedents { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Precedent entity
            modelBuilder.Entity<Precedent>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Citation).IsRequired().HasMaxLength(500);
                entity.Property(p => p.Summary).IsRequired().HasMaxLength(2000);
                entity.Property(p => p.Jurisdiction).IsRequired().HasMaxLength(100);
                entity.Property(p => p.CaseType).HasMaxLength(50);
                entity.Property(p => p.RelevanceScore).HasPrecision(3, 2);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(p => p.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(p => p.Jurisdiction);
                entity.HasIndex(p => p.Year);
                entity.HasIndex(p => p.CaseType);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Precedent>().HasData(
                new Precedent
                {
                    Id = 1,
                    Citation = "Brown v. Board of Education, 347 U.S. 483 (1954)",
                    Summary = "Landmark decision declaring state laws establishing separate public schools for black and white students to be unconstitutional.",
                    Jurisdiction = "United States Supreme Court",
                    Year = 1954,
                    CaseType = "Civil Rights",
                    RelevanceScore = 0.95,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Precedent
                {
                    Id = 2,
                    Citation = "Miranda v. Arizona, 384 U.S. 436 (1966)",
                    Summary = "Established that suspects must be informed of their rights before interrogation, including the right to remain silent and right to an attorney.",
                    Jurisdiction = "United States Supreme Court",
                    Year = 1966,
                    CaseType = "Criminal",
                    RelevanceScore = 0.90,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Precedent
                {
                    Id = 3,
                    Citation = "Roe v. Wade, 410 U.S. 113 (1973)",
                    Summary = "Established constitutional right to abortion under the Due Process Clause of the Fourteenth Amendment.",
                    Jurisdiction = "United States Supreme Court",
                    Year = 1973,
                    CaseType = "Constitutional",
                    RelevanceScore = 0.85,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Precedent
                {
                    Id = 4,
                    Citation = "Marbury v. Madison, 5 U.S. 137 (1803)",
                    Summary = "Established the principle of judicial review, allowing courts to declare laws unconstitutional.",
                    Jurisdiction = "United States Supreme Court",
                    Year = 1803,
                    CaseType = "Constitutional",
                    RelevanceScore = 0.92,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Precedent
                {
                    Id = 5,
                    Citation = "Citizens United v. FEC, 558 U.S. 310 (2010)",
                    Summary = "Held that corporations and unions have the same political speech rights as individuals under the First Amendment.",
                    Jurisdiction = "United States Supreme Court",
                    Year = 2010,
                    CaseType = "Corporate",
                    RelevanceScore = 0.80,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}