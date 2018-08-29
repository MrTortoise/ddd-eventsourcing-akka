namespace NTier_CQS.DomainCommandHandlers.CustomerExperience
{
    public class CreateOrderCommand 
    {
        public CreateOrderCommand(int customerId, int basketId, Money cost)
        {
            CustomerId = customerId;
            BasketId = basketId;
            Cost = cost;
        }

        public int CustomerId { get; }
        public int BasketId { get; }
        public Money Cost { get; private set; }
        public string Name => "CreateOrder";
    }
}