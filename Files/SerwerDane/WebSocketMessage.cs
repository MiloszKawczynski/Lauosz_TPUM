namespace SerwerDane
{
    public static class WebSocketCommands
    {
        public const string Purchase = "PURCHASE";
        public const string Discount = "DISCOUNT";
    }

    public class PurchaseRequest
    {
        public int PlantId { get; set; }
    }
    public class DiscountNotification
    {
        public float DiscountValue { get; set; }
    }
}