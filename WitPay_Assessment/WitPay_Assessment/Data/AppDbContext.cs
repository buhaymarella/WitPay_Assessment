using Microsoft.EntityFrameworkCore;
using WitPay_Assessment.Entity;
namespace WitPay_Assessment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Pizza> Pizzas { get; set; } 
        public DbSet<Toppings> Toppings { get; set; }
        
    }
}

