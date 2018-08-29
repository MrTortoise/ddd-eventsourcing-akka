using System;

namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public class Order
    {
        public void Pick(int warehouseEmployeeId, DateTime updateTime, bool successfullyPicked, string failureReason)
        {
            throw new NotImplementedException();
        }

        public void Ship(int warehouseEmployeeId, DateTime updateTime, ShippingInfo shippingInfo)
        {
            throw new NotImplementedException();
        }
    }
}