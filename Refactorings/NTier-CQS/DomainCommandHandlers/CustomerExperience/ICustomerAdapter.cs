namespace NTier_CQS.DomainCommandHandlers.CustomerExperience
{
    public interface ICustomerAdapter
    {
        Customer GetCustomer(int customerId);
    }
}