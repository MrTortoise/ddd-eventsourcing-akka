namespace EventSourcing
{
    public class AddItemToBasketCommand
    {
        public string AccountName { get; }
        public string Sku { get; }
        public int Quantity { get; }
        public double Cost { get; }

        public AddItemToBasketCommand(string accountName, string sku, int quantity, double cost)
        {
            AccountName = accountName;
            Sku = sku;
            Quantity = quantity;
            Cost = cost;
        }
    }
}