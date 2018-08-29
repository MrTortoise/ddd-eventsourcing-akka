using System.Collections.Generic;


namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public class PickOrderService
    {
        private readonly IGetOrderAdapter _getOrderAdapter;

        public PickOrderService(IGetOrderAdapter getOrderAdapter)
        {
            _getOrderAdapter = getOrderAdapter;
        }

        public void PickOrder(PickOrderCommand pickOrderCommand)
        {
            var order = _getOrderAdapter.GetOrder(pickOrderCommand.OrderId);
            order.Pick(pickOrderCommand.WarehouseEmployeeId, pickOrderCommand.UpdateTime, pickOrderCommand.SuccessfullyPicked, pickOrderCommand.FailureReason);
        }
    }
}
