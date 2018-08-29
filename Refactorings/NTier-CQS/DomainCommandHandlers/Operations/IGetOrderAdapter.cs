namespace NTier_CQS.DomainCommandHandlers.Operations
{
    public interface IGetOrderAdapter
    {
        Order GetOrder(int orderId);
    }
}