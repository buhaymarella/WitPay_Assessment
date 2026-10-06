namespace WitPay_Assessment.Commands.ToppingsCommands
{
    public class GetAllToppingsCommand
    {
        public GetAllToppingsCommand() { }

        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public string? Term { get; set; }
    }
}
