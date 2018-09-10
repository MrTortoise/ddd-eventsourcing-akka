using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Tracing;
using Xunit;

namespace EventSourcing
{
    public class CustomerExperience_CreateAnOrder
    {
        private const string CustomerName = "customerName";
        private const string Email = "email";
        private const string Password = "password";
        private const string AccountName = "accountName";
        private const string Sku = "sku";

        private readonly AccountName _accountName = new AccountName(AccountName);
        private readonly EventStore _eventStore = new EventStore();
        private readonly Func<DateTime> _dateTimeSource = () => DateTime.Now;
        private readonly Func<AccountName, Customer> _customerLoader;
        private readonly Func<AccountName, IEnumerable<IEvent>> _eventLoader;
        private readonly Func<string, string> _passwordHasher = s => s;

        private readonly RegisterCustomerAccountService _registerCustomerAccountService;
        private readonly AddItemToCustomerBasketService _addItemToBasketService;
        private readonly AddPaymentTypeService _addPaymentTypeService;
        

        public CustomerExperience_CreateAnOrder()
        {
            _eventLoader = accountName => _eventStore.LoadEventStream(Customer.CustomerStream(accountName));
            _customerLoader = Customer.BuildLoadCustomer(_eventLoader, new PaymentMethodFactory());

            Action<AccountName, IEvent> eventWriter = (name, @event) =>
                _eventStore.WriteEvent(Customer.CustomerStream(name), @event);

            _registerCustomerAccountService =
                new RegisterCustomerAccountService(
                    eventWriter, _dateTimeSource, _passwordHasher, _customerLoader);
            
            _addItemToBasketService = new AddItemToCustomerBasketService(eventWriter, _dateTimeSource, _customerLoader);
            new PlaceOrderService(eventWriter, _dateTimeSource, _customerLoader);
            _addPaymentTypeService =
                new AddPaymentTypeService(eventWriter, _dateTimeSource, _customerLoader);
        }


        [Fact]
        public void RegisterACustomerAccount()
        {
            _registerCustomerAccountService.Call(
                new RegisterCustomerAccountCommand(CustomerName, AccountName, Email, Password));

            Customer customer = _customerLoader(_accountName);
            Assert.Equal(_accountName.Value, customer.AccountName.Value);
            Assert.Equal(Email, customer.Email);
            Assert.Equal(CustomerName, customer.Name);
        }

        [Fact]
        public void CanAddPaymentTypeToAccountItIsDefault()
        {
            _registerCustomerAccountService.Call(
                new RegisterCustomerAccountCommand(CustomerName, AccountName, Email, Password));

            _addPaymentTypeService.Call(new AddNewPaymentTypeCommand(AccountName, AlwaysPayPaymentProvider.ProviderName,
                "Personal",
                new Dictionary<string, string>()
                {
                    {"specificData", "value"}
                }));

            Customer customer = _customerLoader(_accountName);
            Assert.Single(customer.PaymentMethods);
        }

        [Fact]
        public void AsARegisteredCustomerCanAddItemToBasket()
        {
            _registerCustomerAccountService.Call(
                new RegisterCustomerAccountCommand(CustomerName, AccountName, Email, Password));
            _addItemToBasketService.Call(new AddItemToBasketCommand(AccountName, Sku, 3, 2.34));

            Customer customer = _customerLoader(_accountName);
            int itemsInBasket = customer.Basket.TotalItems;
            Assert.Equal(3, itemsInBasket);
        }

//
//        [Fact]
//        public void WhenPlaceOrderBasketIsEmptyAndUserHasOrder()
//        {
//            _registerCustomerAccountService.Call(
//                new RegisterCustomerAccountCommand(CustomerName, AccountName, Email, Password));
//            _addItemToBasketService.Call(new AddItemToBasketCommand(AccountName, Sku, 3, 2.34));
//            _placeOrderService.Call(new PlaceOrderCommand(AccountName, "paymentMethodId"));
//
//            Customer customer = _customerLoader(_accountName);
//            int totalOrders = customer.Orders.Count;
//            Assert.Equal(1, totalOrders);
//        }
    }
}

public class PlaceOrderCommand
{
    public string AccountName { get; }
    public string PaymentMethod { get; }

    public PlaceOrderCommand(string accountName, string paymentMethod)
    {
        AccountName = accountName;
        PaymentMethod = paymentMethod;
    }
}