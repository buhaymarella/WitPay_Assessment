namespace WitPay_Assessment.DTO
{
    public class PizzaModel
    {
        public int Id { get; set; }
        public string PizzaName { get; set; }
        public List<ToppingModel> Toppings { get; set; }
    }
}
