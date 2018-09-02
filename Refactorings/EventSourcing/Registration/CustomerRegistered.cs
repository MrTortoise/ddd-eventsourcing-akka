using System;

namespace EventSourcing
{
    public class CustomerRegistered : IEvent
    {
        public DateTime CreatedAt { get; }
        public int PId { get; }
        public AccountName AccountName { get; }
        public string Name { get; }
        public string Email { get; }
        public string Password { get; }


        public CustomerRegistered(DateTime createdAt, AccountName accountName, string name, string email,
            string password)
        {
            CreatedAt = createdAt;
            AccountName = accountName;
            Name = name;
            Email = email;
            Password = password;
            PId = System.Diagnostics.Process.GetCurrentProcess().Id;
        }
    }
}