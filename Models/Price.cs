namespace RealTimePrices.Models
{
    public class Price
    {
        public Currency Currency { get; init; }
        public decimal Value { get; init; }

        public Price(Currency currency, decimal value)
        {
            Currency = currency;
            Value = value;
        }
    }
}
