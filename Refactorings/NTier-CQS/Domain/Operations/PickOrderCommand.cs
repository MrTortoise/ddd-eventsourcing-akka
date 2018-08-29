using System;

namespace NTier_CQS.Domain.Operations
{
    public class PickOrderCommand
    {
        public PickOrderCommand(Order.Id orderId, WarehouseEmployee.Id warehouseEmployeeId, DateTime updateTime, bool successfullyPicked, string failureReason)
        {
            OrderId = orderId;
            WarehouseEmployeeId = warehouseEmployeeId;
            UpdateTime = updateTime;
            SuccessfullyPicked = successfullyPicked;
            FailureReason = failureReason;
        }

        public Order.Id OrderId { get; }
        public WarehouseEmployee.Id WarehouseEmployeeId { get; }
        public DateTime UpdateTime { get; }
        public bool SuccessfullyPicked { get; }  // this to me is a sign that something is wrong.
        public string FailureReason { get; }
    }
}