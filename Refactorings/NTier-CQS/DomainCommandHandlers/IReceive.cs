namespace NTier_CQS.DomainCommandHandlers
{
    public interface IReceive<in T>
    {
        void Receive(T c);
    }

}