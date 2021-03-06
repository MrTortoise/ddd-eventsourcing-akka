﻿using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace NTier_CQS.Anemic
{
    public class ShippedOrderPackages
    {
    }

    public class BasketRepository
    {
        public Basket GetBasket(int basket)
        {
            throw new NotImplementedException();
        }
    }

    public class Basket
    {
        public class Id
        {
        }

        public IEnumerable<Item> Items { get; private set; }

        public class Item
        {
            public object Id { get; private set; }
        }
    }

    public class CustomerRepository
    {
        public Customer GetCustomer(int customerId)
        {
            throw new NotImplementedException();
        }
    }

    public class OrderRepository
    {
        public Order CreateOrder(Customer customerId, IEnumerable<object> basket, decimal cost)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrderStatus(int orderId, string reason, string orderStatus)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetOrdersForCustomer(int customerId)
        {
            throw new NotImplementedException();
        }

        public List<ShippedOrderPackages> GetShippedOrders(DateTime today)
        {
            throw new NotImplementedException();
        }

        public Order NewOrder()
        {
            throw new NotImplementedException();
        }

        public void Save(Order order)
        {
            throw new NotImplementedException();
        }
    }

    public class Customer
    {
        public SomeORMCollectionThatsInOurDomainModel Orders { get; set; }
        public Customer Id { get; private set; }


        /// <summary>
        /// Because we do various things at various points we need to make sure that a customer is in a valid state
        /// Eg It might be inbetween guest and real account status.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }

    public class SomeORMCollectionThatsInOurDomainModel
    {
        public List<Order> Load()
        {
            throw new NotImplementedException();
        }
    }

    public class Order
    {
        public Customer CustomerId { get; set; }
        public IEnumerable<object> Items { get; set; }
    }
}