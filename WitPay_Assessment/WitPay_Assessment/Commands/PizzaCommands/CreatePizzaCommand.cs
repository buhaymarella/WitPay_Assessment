namespace WitPay_Assessment.Commands.PizzaCommands
{
    public class CreatePizzaCommand
    {
        public CreatePizzaCommand() { }

        public string PizzaName { get; set; }
        public List<int> ToppingIds { get; set; }
    }
}
