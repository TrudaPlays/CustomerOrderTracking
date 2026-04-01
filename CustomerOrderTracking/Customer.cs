namespace CustomerOrderTracking
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        //naviation property
        public List<Order> Orders { get; set; } = new();
    }
}