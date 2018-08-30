using System;

namespace NTier_CQS.DomainConnascence.Operations
{
    public interface IGetOrderAdapter
    {
        Order GetOrder(int orderId);
    }
}