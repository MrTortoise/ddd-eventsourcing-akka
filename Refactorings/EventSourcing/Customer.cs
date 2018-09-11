using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Linq;

namespace EventSourcing
{
    public class Customer
    {
        public static readonly Customer None = new Customer();
        
        private readonly RegisteredCustomer _registeredCustomer;
        private readonly IPaymentMethodFactory _paymentMethodFactory;

        public AccountName AccountName => _registeredCustomer.AccountName;
        public string Name => _registeredCustomer.Name;
        public string Email => _registeredCustomer.Email;
        public DateTime CreatedAt  => _registeredCustomer.CreatedAt;
        
        public Basket Basket { get; }
        public ImmutableList<Order> Orders { get; }
        public ImmutableDictionary<string, IPaymentMethod> PaymentMethods { get; }

        public static Func<AccountName, Customer> BuildLoadCustomer(Func<AccountName, IEnumerable<IEvent>> eventSource,
            IPaymentMethodFactory paymentMethodFactory)
        {
            return accountName => Load(accountName, eventSource, paymentMethodFactory);
        }

        private static Customer Load(AccountName accountName, Func<AccountName, IEnumerable<IEvent>> eventSource, IPaymentMethodFactory paymentMethodFactory)
        {
            IEnumerable<dynamic> events = eventSource(accountName);
        
   
            if (!events.Any())
            {
                return None;
            }

            var customer = new Customer(RegisteredCustomer.None, paymentMethodFactory);
            return events.Aggregate(customer, (c, e) => c.Apply(e));
        }

        public static void RegisterAccount(AccountName accountName, string customerName, Email email,
            string password, Func<string, string> passwordHasher, DateTime now, Action<AccountName, IEvent> eventWriter)
        {
            eventWriter(accountName,
                new CustomerRegistered(now, accountName, customerName, email.Value, passwordHasher(password)));
        }

        private Customer()
        {
            _registeredCustomer = RegisteredCustomer.None;
        }

        private Customer(RegisteredCustomer registeredCustomer, Basket basket,
            ImmutableDictionary<string, IPaymentMethod> paymentMethods, IPaymentMethodFactory paymentMethodFactory)
            : this(registeredCustomer, paymentMethodFactory)
        {
            Basket = basket;
            PaymentMethods = paymentMethods;
        }

        private Customer(RegisteredCustomer registeredCustomer, IPaymentMethodFactory paymentMethodFactory)
        {
            _registeredCustomer = registeredCustomer;
            _paymentMethodFactory = paymentMethodFactory;
      
            Basket = Basket.Empty;
            PaymentMethods = ImmutableDictionary<string, IPaymentMethod>.Empty;
        }


        private Customer Apply(CustomerRegistered e)
        {
            return new Customer(new RegisteredCustomer(e.AccountName, e.Name, e.Email, e.CreatedAt),
                _paymentMethodFactory);
        }

        private Customer Apply(ItemAddedToBasket e)
        {
            var basket = Basket.AddItemToBasket(e.Sku, e.Quantity, e.Cost, e.AddedAt);
            return new Customer(_registeredCustomer, basket, PaymentMethods, _paymentMethodFactory);
        }

        private Customer Apply(PaymentMethodAdded e)
        {
            var paymentMethod = _paymentMethodFactory.GetProvider(e.TypeName, e.ProviderSpecificData);
            var methods = PaymentMethods.Add(paymentMethod.Name, paymentMethod);
            return new Customer(_registeredCustomer, Basket, methods, _paymentMethodFactory); 
        }

        public void AddItemToBasket(SKU sku, Money cost, int quantity, DateTime addedAt, Action<AccountName, IEvent> eventWriter)
        {
            
            eventWriter(AccountName, new ItemAddedToBasket(sku, cost, quantity, addedAt));
        }

        public void PlaceOrder(Action<AccountName, IEvent> eventWriter)
        {
        }

        /// <summary>
        /// sure it sa dependency
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string Hash(string password)
        {
            return password;
        }

        public static string CustomerStream(AccountName accountName)
        {
            return $"customer-{accountName.Value}";
        }

        public void AddNewPaymentMethod(string customersPaymentMethodName, string typeName,
            Dictionary<string, string> providerSpecificData, Func<DateTime> dateTimeSource, Action<AccountName, IEvent> eventWriter)
        {
            if (PaymentMethods.ContainsKey(customersPaymentMethodName))
                throw new PaymentMethodNameAlreadyInUseException(customersPaymentMethodName);
            
            eventWriter(AccountName, new PaymentMethodAdded(customersPaymentMethodName, typeName,  providerSpecificData, dateTimeSource()));
        }
    }

    public class Email
    {
        public Email(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class PaymentMethod
    {
    }

    public class Order
    {
    }

    public class Basket
    {
        public static readonly Basket Empty = new Basket();

        public ImmutableList<BasketItem> Items { get; } = ImmutableList<BasketItem>.Empty;

        private Basket()
        {
        }

        private Basket(ImmutableList<BasketItem> items, BasketItem basketItem)
        {
            Items = items.Add(basketItem);
        }

        public int TotalItems => Items.Aggregate(0, (c, i) => c + i.Quantity);

        public Basket AddItemToBasket(SKU sku, int quantity, Money cost, DateTime addedAt)
        {
            return new Basket(Items, new BasketItem(sku, quantity, cost, addedAt));
        }

        public class BasketItem
        {
            public SKU Sku { get; }
            public int Quantity { get; }
            public Money Cost { get; }
            public DateTime AddedAt { get; }

            public BasketItem(SKU sku, int quantity, Money cost, DateTime addedAt)
            {
                Sku = sku;
                Quantity = quantity;
                Cost = cost;
                AddedAt = addedAt;
            }
        }
    }
}