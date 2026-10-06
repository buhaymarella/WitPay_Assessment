namespace WitPay_Assessment.Commands.PizzaCommands
{
    public class GetAllPizzaCommand
    {
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public string? Term { get; set;} 
    }
}
