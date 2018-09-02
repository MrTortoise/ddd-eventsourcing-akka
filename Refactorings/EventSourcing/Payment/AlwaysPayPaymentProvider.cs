using System.Collections.Generic;

namespace EventSourcing
{
    internal class AlwaysPayPaymentProvider : IPaymentMethod
    {
        public const string ProviderName = "AlwaysPayPaymentProvider";
        
        public AlwaysPayPaymentProvider(Dictionary<string,string> providerSpecificData)
        {
            
        }

        public string Name => ProviderName;
    }
}