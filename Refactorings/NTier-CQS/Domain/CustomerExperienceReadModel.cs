namespace NTier_CQS.Domain.CustomerExperience
{
    public class OrderHistoryReadModel
    {
        private CustomerEntityModelContext _context;

        public OrderHistory GetCustomerOrderHistory(int customerId)
        {
            _context.GetAllOrderHistory(customerId);
        }
    }

    public class OrderHistory
    {
    }

    public class CustomerReadModels
    {

    }


    public class CustomerEntityModelContext
    {
        public void GetAllOrderHistory(int customerId)
        {
            throw new System.NotImplementedException();
        }
    }
}