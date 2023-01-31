namespace PracticeSession3.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int Amount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}