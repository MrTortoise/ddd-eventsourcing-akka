using System;

namespace NTier_CQS.Domain.Operations
{
    public class Order
    {
        public OrderPickedResult Pick(WarehouseEmployee.Id warehouseEmployeeId, DateTime updateTime,
            bool successfullyPicked, string failureReason, INotifyOrderPicked pickNotifier)
        {
            throw new NotImplementedException();
        }

        public class Id
        {
        }

        public OrderShippedResult Ship(WarehouseEmployee.Id warehouseEmployeeId, DateTime updateTime,
            ShippingInfo shippingInfo, INotifyOrderShipped shippedNotifier)
        {
            throw new NotImplementedException();
        }

        public OrderPickedResult CanOrderBePicked()
        {
            throw new NotImplementedException();
        }
    }
}