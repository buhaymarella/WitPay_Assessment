namespace WitPay_Assessment.Entity
{
    public class Toppings : BaseEntity
    {
        private IList<Pizza> _pizzas = new List<Pizza>();
        public string ToppingName { get; set; }
        public virtual IEnumerable<Pizza> Pizzas
        {
            get { return _pizzas; }
            set { _pizzas = (IList<Pizza>)value; }
        }
    }
}
