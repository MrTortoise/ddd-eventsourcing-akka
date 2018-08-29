using System;

namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public class PickOrderCommand
    {
        public PickOrderCommand(int orderId, int warehouseEmployeeId, DateTime updateTime, bool successfullyPicked, string failureReason)
        {
            OrderId = orderId;
            WarehouseEmployeeId = warehouseEmployeeId;
            UpdateTime = updateTime;
            SuccessfullyPicked = successfullyPicked;
            FailureReason = failureReason;
        }

        public int OrderId { get; }
        public int WarehouseEmployeeId { get; }
        public DateTime UpdateTime { get; }
        public bool SuccessfullyPicked { get; }
        public string FailureReason { get; }
    }
}