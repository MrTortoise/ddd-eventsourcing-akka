namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public class PickOrderService : IReceive<PickOrderCommand>
    {
        private readonly IGetOrderAdapter _getOrderAdapter;
        private readonly INotifyOrderPicked _notifier;

        public PickOrderService(IGetOrderAdapter getOrderAdapter, INotifyOrderPicked notifier)
        {
            _getOrderAdapter = getOrderAdapter;
            _notifier = notifier;
        }

        public void Receive(PickOrderCommand c)
        {
            var order = _getOrderAdapter.GetOrder(c.OrderId);
            order.Pick(c.WarehouseEmployeeId, c.UpdateTime, c.SuccessfullyPicked, c.FailureReason, _notifier);
        }
    }
}
