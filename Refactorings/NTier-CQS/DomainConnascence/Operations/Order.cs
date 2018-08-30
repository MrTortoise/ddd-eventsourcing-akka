using System;

namespace NTier_CQS.DomainConnascence.Operations
{
    public class Order
    {
        public void OrderPicked(int warehouseEmployeeId, DateTime updateTime, bool successfullyPicked, string failureReason)
        {
            throw new NotImplementedException();
        }

        public void OrderShipped(int warehouseEmployeeId, DateTime updateTime, string failureReason, string shippingNumber, bool sucessfullyShipped)
        {
            throw new NotImplementedException();
        }
    }
}