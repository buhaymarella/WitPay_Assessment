using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WitPay_Assessment.Commands.PizzaCommands;
using WitPay_Assessment.Data;
using WitPay_Assessment.DTO;
using WitPay_Assessment.Entity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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

        public async Task<Result> CreatePizzaAsync(CreatePizzaCommand cmd)
        {
            var result = new Result();
            try
            {
                var pizza = new Pizza();
                var toppingIds = cmd.ToppingIds.Distinct().ToList();

                if (cmd.PizzaName.IsNullOrEmpty())
                    return result.Fail("Pizza name is required.");

                var isPizzaExist = await _context.Pizzas
                        .Where(p => p.PizzaName == cmd.PizzaName)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);

                if (isPizzaExist != null)
                    return result.Fail("Pizza with the same name already exists.");

                var toppings = await _context.Toppings
                        .Where(t => toppingIds.Contains(t.Id))
                        .ToListAsync()
                        .ConfigureAwait(false);

                if (!toppings.Any())
                    return result.Fail("No valid toppings found for the provided topping IDs.");


                pizza.PizzaName = cmd.PizzaName;
                pizza.CreatedDate = DateTime.Now;
                pizza.Toppings = toppings;

                _context.Pizzas.Add(pizza);
                var transact = await _context.SaveChangesAsync().ConfigureAwait(false);

                if (transact == 0)
                    return result.Fail("Failed to create pizza.");

                await _context.SaveChangesAsync();


                return result.Success("Pizza created successfully.");
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> UpdatePizzaAsync(UpdatePizzaCommand cmd)
        {
            var result = new Result();
            try
            {
                var toppingIds = cmd.ToppingIds.Distinct().ToList();
                if (cmd.PizzaName.IsNullOrEmpty())
                    return result.Fail("Pizza name is required.");

                if (cmd.Id <= 0)
                    return result.Fail("Invalid pizza ID.");

                var isPizzaExist = await _context.Pizzas
                        .Where(p => p.Id == cmd.Id)
                        .Include(pizza => pizza.Toppings)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                
                if (isPizzaExist == null)
                    return result.Fail("Pizza not found.");

                var toppings = await _context.Toppings
                        .Where(t => toppingIds.Contains(t.Id))
                        .ToListAsync()
                        .ConfigureAwait(false);

                if (!toppings.Any())
                    return result.Fail("No valid toppings found for the provided topping IDs.");

                isPizzaExist.PizzaName = cmd.PizzaName;
                isPizzaExist.UpdatedDate = DateTime.UtcNow;

                var currentToppings = (IList<Toppings>)isPizzaExist.Toppings;

                foreach (var t in currentToppings.Where(t => !cmd.ToppingIds.Contains(t.Id)).ToList())
                    currentToppings.Remove(t);

                foreach (var t in toppings.Where(n => !currentToppings.Any(c => c.Id == n.Id)))
                    currentToppings.Add(t);

                _context.Pizzas.Update(isPizzaExist);
                var transact = await _context.SaveChangesAsync()
                    .ConfigureAwait(false);

                if (transact == 0)
                    return result.Fail("Failed to update pizza.");

                return result.Success("Pizza updated successfully.");
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> DeletePizzaAsync(DeletePizzaCommand cmd)
        {
            var result = new Result();
            try
            {
                if (!cmd.PizzaIds.Any())
                    return result.Fail("Id is required");

                foreach(var id in cmd.PizzaIds)
                {
                    if (id <= 0 || id == null)
                        return result.Fail("Invalid pizza ID.");

                    var isPizzaExist = await _context.Pizzas
                            .Where(p => p.Id == id)
                            .Include(pizza => pizza.Toppings)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

                    if (isPizzaExist == null)
                        return result.Fail("Pizza not found.");

                    _context.Pizzas.Remove(isPizzaExist);
                    var transact = await _context.SaveChangesAsync()
                        .ConfigureAwait(false);

                    if (transact == 0)
                        return result.Fail("Failed to delete pizza.");
                }
                return result.Success("Pizza deleted successfully.");
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> GetAllPizzaAsync(GetAllPizzaCommand cmd)
        {
            var result = new Result();
            try
            {
                int pageSize = cmd.PageSize ?? 10;
                int pageIndex = cmd.PageIndex ?? 1;

                var pizzaQry = _context.Pizzas
                    .Include(pizza => pizza.Toppings)
                    .AsQueryable();

                if (!cmd.Term.IsNullOrEmpty())
                {
                    pizzaQry = pizzaQry
                        .Where(x => x.PizzaName.Contains(cmd.Term));
                }

                var totalRecords = await pizzaQry.CountAsync();

                var pizzas = await pizzaQry
                    .OrderBy(pizza => pizza.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pizzaModels = pizzas.Select(pizza => new PizzaModel
                {
                    Id = pizza.Id,
                    PizzaName = pizza.PizzaName ?? "",
                    Toppings = pizza.Toppings.Select(x => new ToppingModel
                    {
                        Id = x.Id,
                        TopppingName = x.ToppingName ?? ""
                    }).ToList()
                }).ToList();

                return result.Success("Data Fetched Successfully", pizzaModels);
            }catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }
        public async Task<Result> GetPizzaByIdAsync(GetPizzaByIdCommand cmd)
        {
            var result = new Result();
            try
            {
                if (cmd.Id == 0 || cmd.Id == null)
                    return result.Fail("Id is required");

                var pizzaQry = await _context.Pizzas
                    .Include(x => x.Toppings)
                    .Where(x => x.Id == cmd.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (pizzaQry == null)
                    return result.Fail("Pizza does not exist");

                var pizzaModels = new PizzaModel()
                {
                    Id = pizzaQry.Id,
                    PizzaName = pizzaQry.PizzaName ?? "",
                    Toppings = pizzaQry.Toppings.Select(x => new ToppingModel
                    {
                        Id = x.Id,
                        TopppingName = x.ToppingName ?? ""
                    }).ToList()
                };

                return result.Success("Data Fetched Successfully", pizzaModels);
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }
    }
}
