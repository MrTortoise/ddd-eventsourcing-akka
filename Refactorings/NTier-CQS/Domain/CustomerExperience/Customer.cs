using System;

namespace NTier_CQS.Domain.CustomerExperience
{
    public class Customer
    {
        public class Id
        {
        }

        public CreateOrderResult CreateOrder(Basket.Id basketId, Money cost, BasketAdapter basketAdapter,
            IOrderCreatedNotifier notifier)
        {
            throw new NotImplementedException();
        }
    }
}