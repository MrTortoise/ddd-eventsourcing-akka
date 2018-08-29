namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public class ShipOrderService
    {
        private readonly IGetOrderAdapter _getOrderAdapter;

        public ShipOrderService(IGetOrderAdapter getOrderAdapter)
        {
            _getOrderAdapter = getOrderAdapter;
        }

        public void ShipOrder(ShipOrderCommand shipOrderCommand)
        {
            var order = _getOrderAdapter.GetOrder(shipOrderCommand.OrderId);
            order.Ship(shipOrderCommand.WarehouseEmployeeId, shipOrderCommand.UpdateTime, shipOrderCommand.ShippingInfo);
        }
    }
}