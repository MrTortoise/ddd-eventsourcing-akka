using System;

namespace EventSourcing
{
    public class CannotRegisterCustomerAlreadyExistsException : Exception
    {
        public CannotRegisterCustomerAlreadyExistsException(
            RegisterCustomerAccountCommand registerCustomerAccountCommand)
            : base($"Cannot register customer as account name exists: {registerCustomerAccountCommand.AccountName}")
        {
        }
    }
}