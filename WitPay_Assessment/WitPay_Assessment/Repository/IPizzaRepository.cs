using WitPay_Assessment.Commands.PizzaCommands;

namespace WitPay_Assessment.Repository
{
    public interface IPizzaRepository<T> where T : class
    {
        Task<Result> CreatePizzaAsync(CreatePizzaCommand cmd);
        Task<Result> UpdatePizzaAsync(UpdatePizzaCommand cmd);
        Task<Result> DeletePizzaAsync(DeletePizzaCommand cmd);
        Task<Result> GetAllPizzaAsync(GetAllPizzaCommand cmd);
        Task<Result> GetPizzaByIdAsync(GetPizzaByIdCommand cmd);
    }
}
