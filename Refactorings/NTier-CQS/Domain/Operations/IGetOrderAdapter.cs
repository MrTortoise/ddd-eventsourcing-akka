using System;

namespace NTier_CQS.Domain.Operations
{
    public interface IGetOrderAdapter
    {
        Order GetOrder(Order.Id orderId);
    }
}