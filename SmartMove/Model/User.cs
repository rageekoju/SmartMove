namespace SmartMove.Model
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public Currency Currency { get; set; }

        // This method returns the symbol for the selected currency
        public string GetCurrencySymbol()
        {
            return Currency.GetSymbol();
        }
    }

}
