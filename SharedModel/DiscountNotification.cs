namespace SharedModel
{
    [Serializable]
    public struct DiscountNotificationDTO
    {
        public float DiscountValue;
    }

    public class DiscountNotification : IDiscountNotification
    {
        public override float DiscountValue { get; set; }
    }
}
