using System;

namespace EventSourcing
{
    public class AddItemToCustomerBasketService
    {
        private readonly Action<AccountName, IEvent> _eventWriter;
        private readonly Func<DateTime> _dateTimeSource;
        private readonly Func<AccountName, Customer> _loadCustomer;


        public AddItemToCustomerBasketService(Action<AccountName, IEvent> eventWriter, Func<DateTime> dateTimeSource, Func<AccountName, Customer> loadCustomer)
        {
            _eventWriter = eventWriter;
            _dateTimeSource = dateTimeSource;
            _loadCustomer = loadCustomer;
        }

        public void Call(AddItemToBasketCommand c)
        {
            var customer = _loadCustomer(new AccountName(c.AccountName));
            customer.AddItemToBasket(new SKU(c.Sku), new Money(c.Cost), c.Quantity, _dateTimeSource(), _eventWriter);
        }
    }
}