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

        /// <summary>
        /// Look this function creates an order and then returns an order.
        /// We need to query several datastores in turn and then write and requery.
        /// This is a transactional and consistency nightmare.
        /// Of course our 'business logic' is completley muddied with the repository pattern
        /// that we are using to enable us to change database? or repo? what exactly?
        /// </summary>
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
                // because the function is there.
                // what are we really asking? Can this customer make orders?
            }
            
            var basket = _basketRepository.GetBasket(basketId);
            if (basket == null)
            {
                throw new InvalidOperationException($"Basket {basketId} does not exist");
            }

            return _orderRepository.CreateOrder(customer, basket, cost);
        }

        /// <summary>
        /// We are using the same repositories for reading and writing
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<Order> GetCustomersOrderHistory(int customerId)
        {
            return _orderRepository.GetOrdersForCustomer(customerId);
        }

        /// <summary>
        /// But this architecture works really well for reads.
        /// Well if you want to do all those joins everytime its fine.
        /// </summary>
        /// <returns></returns>
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
}