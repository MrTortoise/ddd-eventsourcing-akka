using System;

namespace NTier_CQS.DomainCommandHandlers.CustomerExperience
{
    public class CreateOrderService : IReceive<CreateOrderCommand>
    {
        private readonly ICustomerAdapter _customerAdapter;
        private readonly IBasketAdapter _basketAdapter;
        private readonly ICreateOrderNotifier _notifier;

        public CreateOrderService(ICustomerAdapter customerAdapter, IBasketAdapter basketAdapter,
            ICreateOrderNotifier notifier)
        {
            _customerAdapter = customerAdapter;
            _basketAdapter = basketAdapter;
            _notifier = notifier;
        }

        public void Receive(CreateOrderCommand c)
        {
            var customer = _customerAdapter.GetCustomer(c.CustomerId);
            customer.CreateOrder(c.BasketId, c.Cost, _basketAdapter, _notifier);
        }


        public static Action<CreateOrderCommand> CreateOrdeHandler(ICustomerAdapter customerAdapter,IBasketAdapter basketAdapter,ICreateOrderNotifier notifier)
        {
            return c =>
            {
                var customer = customerAdapter.GetCustomer(c.CustomerId);
                customer.CreateOrder(c.BasketId, c.Cost, basketAdapter, notifier);
            };
        }
    }
}