using System;

namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public class ShipOrderCommand
    {
        public ShipOrderCommand(int orderId, int warehouseEmployeeId, DateTime updateTime, ShippingInfo shippingInfo)
        {
            OrderId = orderId;
            WarehouseEmployeeId = warehouseEmployeeId;
            UpdateTime = updateTime;
            ShippingInfo = shippingInfo;
        }

        public int OrderId { get; }
        public int WarehouseEmployeeId { get; }
        public DateTime UpdateTime { get; }
        public ShippingInfo ShippingInfo { get; }
    }
}