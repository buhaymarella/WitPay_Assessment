using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WitPay_Assessment.Commands.PizzaCommands;
using WitPay_Assessment.Commands.ToppingsCommands;
using WitPay_Assessment.Data;
using WitPay_Assessment.DTO;
using WitPay_Assessment.Entity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace WitPay_Assessment.Repository
{
    public class ToppingsRepository : IToppingsRepository<Toppings>
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ToppingsRepository(AppDbContext context, IConfiguration configuration) : base()
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result> CreateToppingAsync(CreateToppingsCommand cmd)
        {
            var result = new Result();
            try
            {
                var topping = new Toppings();

                if (cmd.ToppingName.IsNullOrEmpty())
                    return result.Fail("Topping name is required.");

                var isToppingExist = await _context.Toppings
                        .Where(t => t.ToppingName == cmd.ToppingName)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);

                if (isToppingExist != null)
                    return result.Fail("Topping with the same name already exists.");

                topping.ToppingName = cmd.ToppingName;
                topping.CreatedDate = DateTime.Now;

                _context.Toppings.Add(topping);
                var transact = await _context.SaveChangesAsync().ConfigureAwait(false);

                if (transact == 0)
                    return result.Fail("Failed to create topping.");

                await _context.SaveChangesAsync();


                return result.Success("Topping created successfully.");
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> UpdateToppingAsync(UpdateToppingsCommand cmd)
        {
            var result = new Result();
            try
            {
                if (cmd.ToppingName.IsNullOrEmpty())
                    return result.Fail("Topping name is required.");

                if (cmd.Id <= 0 || cmd.Id == null)
                    return result.Fail("Invalid topping ID.");

                var isToppingExist = await _context.Toppings
                        .Where(t => t.Id == cmd.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);

                if (isToppingExist == null)
                    return result.Fail("Topping not found.");

                isToppingExist.ToppingName = cmd.ToppingName;
                isToppingExist.UpdatedDate = DateTime.UtcNow;

                _context.Toppings.Update(isToppingExist);
                var transact = await _context.SaveChangesAsync()
                    .ConfigureAwait(false);

                if (transact == 0)
                    return result.Fail("Failed to update topping.");

                return result.Success("Topping updated successfully.");
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> DeleteToppingAsync(DeleteToppingsCommand cmd)
        {
            var result = new Result();
            try
            {
                if (!cmd.Ids.Any())
                    return result.Fail("Id is required");

                foreach (var id in cmd.Ids)
                {
                    if (id <= 0 || id == null)
                        return result.Fail("Invalid topping ID.");

                    var isToppingExist = await _context.Toppings
                            .Where(t => t.Id == id)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

                    if (isToppingExist == null)
                        return result.Fail("Topping not found.");

                    _context.Toppings.Remove(isToppingExist);
                    var transact = await _context.SaveChangesAsync()
                        .ConfigureAwait(false);

                    if (transact == 0)
                        return result.Fail("Failed to delete topping.");
                }
                return result.Success("Topping deleted successfully.");
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> GetAllToppingsAsync(GetAllToppingsCommand cmd)
        {
            var result = new Result();
            try
            {
                int pageSize = cmd.PageSize ?? 10;
                int pageIndex = cmd.PageIndex ?? 1;

                var toppingQry = _context.Toppings
                    .AsQueryable();

                if (!cmd.Term.IsNullOrEmpty())
                {
                    toppingQry = toppingQry
                        .Where(x => x.ToppingName.Contains(cmd.Term));
                }

                var totalRecords = await toppingQry.CountAsync();

                var toppings = await toppingQry
                    .OrderBy(topping => topping.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var toppingModels = toppings.Select(topping => new ToppingModel
                {
                    Id = topping.Id,
                    TopppingName = topping.ToppingName ?? ""
                }).ToList();

                return result.Success("Data Fetched Successfully", toppingModels);
            }catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }
        public async Task<Result> GetToppingByIdAsync(GetToppingsByIdCommand cmd)
        {
            var result = new Result();
            try
            {
                if (cmd.Id == 0 || cmd.Id == null)
                    return result.Fail("Id is required");

                var toppingQry = await _context.Toppings
                    .Where(x => x.Id == cmd.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (toppingQry == null)
                    return result.Fail("Topping does not exist");

                var toppingModels = new ToppingModel()
                {
                    Id = toppingQry.Id,
                    TopppingName = toppingQry.ToppingName ?? ""
                };

                return result.Success("Data Fetched Successfully", toppingModels);
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }
    }
}
