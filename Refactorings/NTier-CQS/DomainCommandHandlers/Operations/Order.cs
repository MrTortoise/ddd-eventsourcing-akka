using System;

namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public class Order
    {
        public void Pick(int warehouseEmployeeId, DateTime updateTime, bool successfullyPicked, string failureReason,
            INotifyOrderPicked notifier)
        {
            throw new NotImplementedException();
        }

        public void Ship(int warehouseEmployeeId, DateTime updateTime, ShippingInfo shippingInfo,
            INotifyOrderShipped notifier)
        {
            throw new NotImplementedException();
        }
    }
}