using System;

namespace EventSourcing
{
    public class PaymentMethodNameAlreadyInUseException : Exception
    {
        public PaymentMethodNameAlreadyInUseException(string customersPaymentMethodName) : base(
            $"Customer attempted to add payment method, but customer already using that method name: {customersPaymentMethodName}")
        {
        }
    }
}