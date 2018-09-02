using System;

namespace EventSourcing
{
    internal class AddPaymentTypeService
    {
        private readonly Action<AccountName, IEvent> _eventWriter;
        private readonly Func<DateTime> _dateTimeSource;
        private readonly Func<AccountName, Customer> _customerLoader;

        public AddPaymentTypeService(Action<AccountName, IEvent> eventWriter, Func<DateTime> dateTimeSource, Func<AccountName, Customer> customerLoader)
        {
            _eventWriter = eventWriter;
            _dateTimeSource = dateTimeSource;
            _customerLoader = customerLoader;
        }

        public void Call(AddNewPaymentTypeCommand c)
        {
            var customer = _customerLoader(new AccountName(c.AccountName));
            customer.AddNewPaymentMethod(c.CustomersPaymentMethodName,c.TypeName, c.ProviderSpecificData, _dateTimeSource,  _eventWriter);
            
        }
    }
}