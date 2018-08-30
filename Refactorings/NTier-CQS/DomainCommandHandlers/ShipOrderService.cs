namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public class ShipOrderService : IReceive<ShipOrderCommand>
    {
        private readonly IGetOrderAdapter _getOrderAdapter;
        private readonly INotifyOrderShipped _notifier;

        public ShipOrderService(IGetOrderAdapter getOrderAdapter, INotifyOrderShipped notifier)
        {
            _getOrderAdapter = getOrderAdapter;
            _notifier = notifier;
        }

        public void Receive(ShipOrderCommand c)
        {
            var order = _getOrderAdapter.GetOrder(c.OrderId);
            order.Ship(c.WarehouseEmployeeId, c.UpdateTime, c.ShippingInfo, _notifier);
        }
    }
}