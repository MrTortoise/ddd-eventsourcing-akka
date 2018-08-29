using System;
using System.Collections.Generic;

namespace NTier_CQS.Anemic
{
    /// <summary>
    /// Its almost like this approach is optomised for queries ....
    /// </summary>
    public class OrdersServiceAnemic
    {
        private readonly OrderRepository _orderRepository;
        private readonly BasketRepository _basketRepository;
        private readonly CustomerRepository _customerRepository;

        public OrdersServiceAnemic(OrderRepository orderRepo, CustomerRepository customerRepo, BasketRepository basketRepository)
        {
            _customerRepository = customerRepo;
            _basketRepository = basketRepository;
            _orderRepository = orderRepo;
        }

        public Order CreateOrder(int customerId, decimal cost, int basketId)
        {
            if (customerId < 1) throw new ArgumentOutOfRangeException(nameof(customerId), customerId, "Customer Id must be set (positive)");
            if(cost<0) throw new ArgumentOutOfRangeException(nameof(cost), cost,  "cost must be >= 0");
            if (basketId<1) throw new ArgumentOutOfRangeException(nameof(basketId), basketId,  "Basket Id must be set (positive");
            
            var customer = _customerRepository.GetCustomer(customerId);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer {customerId} does not exist");
            }

            if (!customer.IsValid())
            {
                
            }
            var basket = _basketRepository.GetBasket(basketId);
            if (basket == null)
            {
                throw new InvalidOperationException($"Basket {basketId} does not exist");
            }

            return _orderRepository.CreateOrder(customer, basket, cost);
        }

        public List<Order> GetCustomersOrderHistory(int customerId)
        {
            return _orderRepository.GetOrdersForCustomer(customerId);
        }

        public List<ShippedOrderPackages> GetTodaysShippedOrders()
        {
            return _orderRepository.GetShippedOrders(DateTime.Today);
        }
        

        /// <summary>
        /// who is calling this and why?
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderStatus"></param>
        /// <param name="reason"></param>
        /// <param name="data"></param>
        public void UpdateOrderStatus(int orderId, string orderStatus, string reason, List<string> data)
        {
            _orderRepository.UpdateOrderStatus(orderId, reason, orderStatus);
        }
    }

    public class ShippedOrderPackages
    {
    }
}