public enum Currency
{
    NPR = 1,
    USD = 2,
    EUR = 3
}

public static class CurrencyExtensions
{
    // Extension method to get the currency symbol
    public static string GetSymbol(this Currency currency)
    {
        switch (currency)
        {
            case Currency.NPR:
                return "NPR";  // Nepalese Rupee
            case Currency.USD:
                return "$";    // US Dollar
            case Currency.EUR:
                return "EUR";    // Indian Rupee symbol
            default:
                return string.Empty;
        }
    }
}
