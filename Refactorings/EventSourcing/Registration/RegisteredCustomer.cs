using System;

namespace EventSourcing
{
    public class RegisteredCustomer
    {
        private const string Empty = "none";
        public static readonly RegisteredCustomer None = new RegisteredCustomer(AccountName.None, Empty, Empty, DateTime.MinValue);
        
        public RegisteredCustomer(AccountName accountName, string name, string email, DateTime createdAt)
        {
            AccountName = accountName;
            Name = name;
            Email = email;
            CreatedAt = createdAt;
        }

        public AccountName AccountName { get; }
        public string Name { get; }
        public string Email { get; }
        public DateTime CreatedAt { get; }
    }
}