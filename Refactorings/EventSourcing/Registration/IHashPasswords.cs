namespace EventSourcing
{
    public interface IHashPasswords
    {
        string Hash(string password);
    }
}