using System;

namespace EventSourcing
{
    internal class PlaceOrderService
    {
        private readonly Action<AccountName, IEvent> _eventWriter;
        private readonly Func<DateTime> _dateTimeSource;
        private readonly Func<AccountName, Customer> _customerLoader;

        public PlaceOrderService(Action<AccountName, IEvent> eventWriter, Func<DateTime> dateTimeSource, Func<AccountName, Customer> customerLoader)
        {
            _eventWriter = eventWriter;
            _dateTimeSource = dateTimeSource;
            _customerLoader = customerLoader;
        }

        public void Call(PlaceOrderCommand c)
        {
            var customer = _customerLoader(new AccountName(c.AccountName));
            customer.PlaceOrder(_eventWriter);
        }
    }
}