using WitPay_Assessment.Commands.PizzaCommands;
using WitPay_Assessment.Commands.ToppingsCommands;

namespace WitPay_Assessment.Repository
{
    public interface IToppingsRepository<T> where T : class
    {
        Task<Result> CreateToppingAsync(CreateToppingsCommand cmd);
        Task<Result> UpdateToppingAsync(UpdateToppingsCommand cmd);
        Task<Result> DeleteToppingAsync(DeleteToppingsCommand cmd);
        Task<Result> GetAllToppingsAsync(GetAllToppingsCommand cmd);
        Task<Result> GetToppingByIdAsync(GetToppingsByIdCommand cmd);
    }
}
