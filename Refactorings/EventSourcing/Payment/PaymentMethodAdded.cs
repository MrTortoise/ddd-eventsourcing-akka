using System;
using System.Collections.Generic;

namespace EventSourcing
{
    public class PaymentMethodAdded : IEvent
    {
        public string Name { get; }
        public string TypeName { get; }
        public Dictionary<string, string> ProviderSpecificData { get; }
        public DateTime AddedAt { get; }

        public PaymentMethodAdded(string name, string typeName, Dictionary<string,string> providerSpecificData, DateTime addedAt)
        {
            Name = name;
            TypeName = typeName;
            ProviderSpecificData = providerSpecificData;
            AddedAt = addedAt;
            PId = System.Diagnostics.Process.GetCurrentProcess().Id;
        }

        public int PId { get; }
    }
}