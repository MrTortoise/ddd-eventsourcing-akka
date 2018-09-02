using System.Collections.Generic;

namespace EventSourcing
{
    public class AddNewPaymentTypeCommand
    {
        public string AccountName { get; }
        public string TypeName { get; }
        public string CustomersPaymentMethodName { get; }
        public Dictionary<string, string> ProviderSpecificData { get; }

        public AddNewPaymentTypeCommand(string accountName, string typeName, string customersPaymentMethodName,
            Dictionary<string, string> providerSpecificData)
        {
            AccountName = accountName;
            TypeName = typeName;
            CustomersPaymentMethodName = customersPaymentMethodName;
            ProviderSpecificData = providerSpecificData;
        }
    }
}