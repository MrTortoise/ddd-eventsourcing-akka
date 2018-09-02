using System.Collections.Generic;

namespace EventSourcing
{
    public interface IPaymentMethodFactory
    {
        IPaymentMethod GetProvider(string typeName, Dictionary<string, string> providerSpecificData);
    }

    public class PaymentMethodFactory : IPaymentMethodFactory
    {
        public IPaymentMethod GetProvider(string typeName, Dictionary<string, string> providerSpecificData)
        {
//            switch (typeName)
//            {
//                case AlwaysPayPaymentProvider.Name:
            return new AlwaysPayPaymentProvider(providerSpecificData);
//                    break;
//            }
        }
    }
}