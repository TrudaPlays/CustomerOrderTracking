namespace CustomerOrderTracking
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        //foreign key + navigation property
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}