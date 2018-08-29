using System;

namespace NTier_CQS.DomainConnascence.CustomerExperience
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

        public void CreateOrder(int customerId, decimal cost, int basketId)
        {
            var customer = _customerAdapter.GetCustomer(customerId);
            customer.CreateOrder(basketId, cost, _basketAdapter);
        }
    }
}


namespace NTier_CQS.DomainConnascence.Operations
{
    public class OrderWarehouseOperationsCheckpointService
    {
        private readonly GetOrderAdapter _getOrderAdapter;

        public OrderWarehouseOperationsCheckpointService(GetOrderAdapter getOrderAdapter)
        {
            _getOrderAdapter = getOrderAdapter;
        }

        public void OrderPicked(int orderId, int warehouseEmployeeId, DateTime updateTime, bool successfullyPicked,
            string failureReason)
        {
            var order = _getOrderAdapter.GetOrder(orderId);
            order.OrderPicked(warehouseEmployeeId, updateTime, successfullyPicked, failureReason);
        }

        public void OrderShipped(int orderId, int warehouseEmployeeId, DateTime updateTime, bool sucessfullyShipped,
            string shippingNumber, string failureReason)
        {
            var order = _getOrderAdapter.GetOrder(orderId);
            order.OrderShipped(warehouseEmployeeId, updateTime, shippingNumber, failureReason, sucessfullyShipped);
        }
    }
}
