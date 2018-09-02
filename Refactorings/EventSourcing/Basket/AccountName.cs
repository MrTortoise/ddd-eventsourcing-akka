namespace EventSourcing
{
    public class AccountName
    {
        public static readonly AccountName None = new AccountName("None");
        
        public string Value { get; }

        public AccountName(string name)
        {
            Value = name;
        }
    }
}