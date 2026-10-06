namespace WitPay_Assessment.Entity
{
    public class Pizza : BaseEntity
    {
        private IList<Toppings> _toppings = new List<Toppings>();
        public Pizza() { }
        public string PizzaName { get; set; }

        public virtual IEnumerable<Toppings> Toppings
        {
            get { return _toppings; }
            set { _toppings = (IList<Toppings>)value; }
        }

    }
}
