using System;

namespace EventSourcing
{
    public class RegisterCustomerAccountService
    {
        private readonly Action<AccountName, IEvent> _eventWriter;
        private readonly Func<DateTime> _dateTimeSource;
        private readonly Func<string, string> _passwordHasher;
        private readonly Func<AccountName, Customer> _customerLoader;

        public RegisterCustomerAccountService(Action<AccountName, IEvent> eventWriter, Func<DateTime> dateTimeSource, Func<string,string> passwordHasher, Func<AccountName, Customer> customerLoader)
        {
            _eventWriter = eventWriter;
            _dateTimeSource = dateTimeSource;
            _passwordHasher = passwordHasher;
            _customerLoader = customerLoader;
        }

        public void Call(RegisterCustomerAccountCommand c)
        {
            var accountName = new AccountName(c.AccountName);
            var customer = _customerLoader(accountName);
            if (customer != Customer.None)
            {
                throw new CannotRegisterCustomerAlreadyExistsException(c);
            }

            Customer.RegisterAccount(accountName, c.Name, c.Email, c.Password, _passwordHasher, _dateTimeSource(), _eventWriter);
        }
    }
}