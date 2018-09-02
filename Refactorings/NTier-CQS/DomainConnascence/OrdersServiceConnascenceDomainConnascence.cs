using System;

namespace NTier_CQS.DomainConnascence.CustomerExperience.Write
{
    public class CreateOrderService
    {
        private readonly CustomerAdapter _customerAdapter;
        private readonly BasketAdapter _basketAdapter;

        public CreateOrderService(CustomerAdapter customerAdapter, BasketAdapter basketAdapter)
        {
            _customerAdapter = customerAdapter;
            _basketAdapter = basketAdapter;
        }

        public void CreateOrder(int customerId, decimal cost, int basketId)
        {
            var customer = _customerAdapter.GetCustomer(customerId);
            customer.CreateOrder(basketId, cost, _basketAdapter);
        }
    }
}

// The point of seperating out the namespaces is that these are very different areas
// they will probably have different values to the business in terms of build:buy:commodity
// or in terms of competative advantage vs amount of work. EG You can buy warehouse operations.
// You can also buy ecommerce systems ...
namespace NTier_CQS.DomainConnascence.Operations.Write
{
    public class OrderPickedService
    {
        private readonly IGetOrderAdapter _getOrderAdapter;

        public OrderPickedService(IGetOrderAdapter getOrderAdapter)
        {
            _getOrderAdapter = getOrderAdapter;
        }

        public void OrderPicked(int orderId, int warehouseEmployeeId, DateTime updateTime, bool successfullyPicked,
            string failureReason)
        {
            var order = _getOrderAdapter.GetOrder(orderId);
            order.OrderPicked(warehouseEmployeeId, updateTime, successfullyPicked, failureReason);
        }
    }

    public class OrderShippedService
    {
        private readonly IGetOrderAdapter _getOrderAdapter;

        public OrderShippedService(IGetOrderAdapter getOrderAdapter)
        {
            _getOrderAdapter = getOrderAdapter;
        }

        public void OrderShipped(int orderId, int warehouseEmployeeId, DateTime updateTime, bool sucessfullyShipped,
            string shippingNumber, string failureReason)
        {
            var order = _getOrderAdapter.GetOrder(orderId);
            order.OrderShipped(warehouseEmployeeId, updateTime, shippingNumber, failureReason,  sucessfullyShipped);
        }
    }


    public class ReadSSomething
    {
        
    }
}
