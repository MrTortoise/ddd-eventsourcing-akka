using System;

namespace NTier_CQS.Domain.Operations
{
    public class ShipOrderCommand
    {
        public ShipOrderCommand(Order.Id orderId, WarehouseEmployee.Id warehouseEmployeeId, DateTime updateTime, ShippingInfo shippingInfo)
        {
            OrderId = orderId;
            WarehouseEmployeeId = warehouseEmployeeId;
            UpdateTime = updateTime;
            ShippingInfo = shippingInfo;
        }

        public Order.Id OrderId { get; }
        public WarehouseEmployee.Id WarehouseEmployeeId { get; }
        public DateTime UpdateTime { get; }
     
        public ShippingInfo ShippingInfo { get; }
    }
}