using System;

namespace NTier_CQS.Domain.Operations
{
    public class Order
    {
        public OrderPickedResult Pick(WarehouseEmployee.Id warehouseEmployeeId, DateTime updateTime, bool successfullyPicked, string failureReason)
        {
            throw new NotImplementedException();
        }

        public class Id
        {
        }

        public OrderShippedResult Ship(WarehouseEmployee.Id warehouseEmployeeId, DateTime updateTime, ShippingInfo shippingInfo)
        {
            throw new NotImplementedException();
        }

        public OrderPickedResult CanOrderBePicked()
        {
            throw new NotImplementedException();
        }
    }
}