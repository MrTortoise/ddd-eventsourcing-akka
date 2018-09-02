namespace EventSourcing
{
    public class RegisterCustomerAccountCommand
    {
        public string Name { get; }
        public string AccountName { get; }
        public string Email { get; }
        public string Password { get; }

        public RegisterCustomerAccountCommand(string name, string accountName, string email, string password)
        {
            Name = name;
            AccountName = accountName;
            Email = email;
            Password = password;
        }
    }
}