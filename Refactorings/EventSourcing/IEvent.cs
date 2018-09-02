namespace EventSourcing
{
    public interface IEvent
    {
        int PId { get; }
    }
}