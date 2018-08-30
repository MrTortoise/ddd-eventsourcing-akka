using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;

namespace NTier_CQS.Domain.CustomerExperience
{
    public class CustomerOrderCreateService
    {
        private readonly CustomerAdapter _customerAdapter;
        private readonly BasketAdapter _basketAdapter;
        private readonly IOrderCreatedNotifier _notifier;

        public CustomerOrderCreateService(CustomerAdapter customerAdapter, BasketAdapter basketAdapter, IOrderCreatedNotifier notifier)
        {
            _customerAdapter = customerAdapter;
            _basketAdapter = basketAdapter;
            _notifier = notifier;
        }

        public CreateOrderResult CreateOrder(CreateOrderCommand createOrderCommand)
        {
            var customer = _customerAdapter.GetCustomer(createOrderCommand.CustomerId);
            return customer.CreateOrder(createOrderCommand.BasketId, createOrderCommand.Cost, _basketAdapter, _notifier);
        }
    }
}


namespace NTier_CQS.Domain.Operations
{
    public class OrderPickedService
    {
        private readonly IGetOrderAdapter _getOrderAdapter;
        private readonly INotifyOrderPicked _pickNotifier;

        public OrderPickedService(IGetOrderAdapter getOrderAdapter, INotifyOrderPicked pickNotifier)
        {
            _getOrderAdapter = getOrderAdapter;
            _pickNotifier = pickNotifier;
        }

        public OrderPickedResult OrderPicked(PickOrderCommand c)
        {
            var order = _getOrderAdapter.GetOrder(c.OrderId);
            
            var canOrderBePicked = order.CanOrderBePicked();
            if (!canOrderBePicked.Result) return canOrderBePicked;
            
            return order.Pick(c.WarehouseEmployeeId, c.UpdateTime, c.SuccessfullyPicked, c.FailureReason, _pickNotifier);
        }
    }

    public class OrderShippedService
    {
        private readonly IGetOrderAdapter _getOrderAdapter;
        private readonly INotifyOrderShipped _shippedNotifier;

        public OrderShippedService(IGetOrderAdapter getOrderAdapter, INotifyOrderShipped shippedNotifier)
        {
            _getOrderAdapter = getOrderAdapter;
            _shippedNotifier = shippedNotifier;
        }

        public OrderShippedResult OrderShipped(ShipOrderCommand c)
        {
            var order = _getOrderAdapter.GetOrder(c.OrderId);
            return order.Ship(c.WarehouseEmployeeId, c.UpdateTime, c.ShippingInfo, _shippedNotifier);
        }
    }
}
