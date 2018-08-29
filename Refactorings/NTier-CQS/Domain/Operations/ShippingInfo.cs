namespace NTier_CQS.Domain.Operations
{
    public class ShippingInfo
    {
        public ShippingInfo(bool sucessfullyShipped, string shippingNumber, string failureReason)
        {
            SucessfullyShipped = sucessfullyShipped;
            ShippingNumber = shippingNumber;
            FailureReason = failureReason;
        }

        public bool SucessfullyShipped { get; }
        public string ShippingNumber { get; }
        public string FailureReason { get; }
    }
}