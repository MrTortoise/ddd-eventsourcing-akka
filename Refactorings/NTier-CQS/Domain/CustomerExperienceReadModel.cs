using System.Collections.Generic;

namespace NTier_CQS.Domain.CustomerExperience
{
    public class OrderHistoryReadModel
    {
        private IOrderHistoryRepository _repo;

        public OrderHistoryReadModel(IOrderHistoryRepository repo)
        {
            _repo = repo;
        }

        public List<Order> GetCustomerOrderHistory(int customerId)
        {
            return _repo.GetAllOrderHistory(customerId);
        }
    }


    /// <summary>
    /// Could be anything (sql, nosql, edge, geopatial)
    /// </summary>
    public interface IOrderHistoryRepository
    {
        List<Order> GetAllOrderHistory(int customerId);
    }

    public class Order
    {
    }
}