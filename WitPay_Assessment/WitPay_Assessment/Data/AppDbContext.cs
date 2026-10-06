using Microsoft.EntityFrameworkCore;
using WitPay_Assessment.Entity;
namespace WitPay_Assessment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Pizza> Pizzas { get; set; } 
        public DbSet<Toppings> Toppings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pizza>()
                .HasMany(p => p.Toppings)
                .WithMany(t => t.Pizzas)
                .UsingEntity<Dictionary<string, object>>(
                    "PizzaToppings",
                    j => j.HasOne<Toppings>().WithMany().HasForeignKey("ToppingsId"),
                    j => j.HasOne<Pizza>().WithMany().HasForeignKey("PizzasId"),
                    j =>
                    {
                        j.HasKey("PizzasId", "ToppingsId");
                        j.Property<DateTime?>("CreatedDate");
                        j.Property<DateTime?>("UpdatedDate");
                    });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditDates();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditDates();
            return base.SaveChanges();
        }

        private void ApplyAuditDates()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                var created = entry.Metadata.FindProperty("CreatedDate");
                var updated = entry.Metadata.FindProperty("UpdatedDate");

                if (entry.State == EntityState.Added && created != null)
                {
                    entry.Property("CreatedDate").CurrentValue = now;
                }
                else if (entry.State == EntityState.Modified && updated != null)
                {
                    entry.Property("UpdatedDate").CurrentValue = now;
                    if (created != null)
                        entry.Property("CreatedDate").IsModified = false;
                }
            }
        }
    }
}

