namespace NTier_CQS.DomainCommandHandlers.CustomerExperience
{
    public class CreateOrderService : IReceive<CreateOrderCommand>
    {
        private readonly ICustomerAdapter _customerAdapter;
        private readonly IBasketAdapter _basketAdapter;

        public CreateOrderService(ICustomerAdapter customerAdapter, IBasketAdapter basketAdapter)
        {
            _customerAdapter = customerAdapter;
            _basketAdapter = basketAdapter;
        }

        public void Receive(CreateOrderCommand command)
        {
            var customer = _customerAdapter.GetCustomer(command.CustomerId);
            customer.CreateOrder(command.BasketId, command.Cost, _basketAdapter);
        }
    }
}