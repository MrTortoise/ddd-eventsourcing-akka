namespace NTier_CQS.Domain.CustomerExperience
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand(Customer.Id customerId, Money cost, Basket.Id basketId)
        {
            CustomerId = customerId;
            Cost = cost;
            BasketId = basketId;
        }

        public Customer.Id CustomerId { get; }
        public Money Cost { get; }
        public Basket.Id BasketId { get; }
    }
}