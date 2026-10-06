using Microsoft.EntityFrameworkCore;
using WitPay_Assessment.Data;
using WitPay_Assessment.Entity;
namespace WitPay_Assessment.Repository
{
    public class PizzaRepository : IPizzaRepository<Pizza>
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PizzaRepository(AppDbContext context, IConfiguration configuration) : base()
        {
            _context = context;
            _configuration = configuration;
        }

    }
}
