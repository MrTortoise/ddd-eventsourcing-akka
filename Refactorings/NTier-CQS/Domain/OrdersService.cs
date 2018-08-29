using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;

namespace NTier_CQS.Domain.CustomerExperience
{
    public class CustomerOrderCreateService
    {
        private readonly CustomerAdapter _customerAdapter;
        private readonly BasketAdapter _basketAdapter;

        public CustomerOrderCreateService(CustomerAdapter customerAdapter, BasketAdapter basketAdapter)
        {
            _customerAdapter = customerAdapter;
            _basketAdapter = basketAdapter;
        }

        public CreateOrderResult CreateOrder(CreateOrderCommand createOrderCommand)
        {
            var customer = _customerAdapter.GetCustomer(createOrderCommand.CustomerId);
            return customer.CreateOrder(createOrderCommand.BasketId, createOrderCommand.Cost, _basketAdapter);
        }
    }
}


namespace NTier_CQS.Domain.Operations
{
    public class OrderWarehouseOperationsCheckpointService
    {
        private readonly GetOrderAdapter _getOrderAdapter;

        public OrderWarehouseOperationsCheckpointService(GetOrderAdapter getOrderAdapter)
        {
            _getOrderAdapter = getOrderAdapter;
        }

        public OrderPickedResult OrderPicked(PickOrderCommand pickOrderCommand)
        {
            var order = _getOrderAdapter.GetOrder(pickOrderCommand.OrderId);
            
            var canOrderBePicked = order.CanOrderBePicked();
            if (!canOrderBePicked.Result) return canOrderBePicked;
            
            return order.Pick(pickOrderCommand.WarehouseEmployeeId, pickOrderCommand.UpdateTime, pickOrderCommand.SuccessfullyPicked, pickOrderCommand.FailureReason);
        }

        public OrderShippedResult OrderShipped(ShipOrderCommand shipOrderCommand)
        {
            var order = _getOrderAdapter.GetOrder(shipOrderCommand.OrderId);
            return order.Ship(shipOrderCommand.WarehouseEmployeeId, shipOrderCommand.UpdateTime, shipOrderCommand.ShippingInfo);
        }
    }
}
