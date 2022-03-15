namespace RealTimePrices.Models
{
    public class Paper
    {
        public string Figi { get; init; }
        public string Name { get; init; }
        public string Ticker { get; init; }
        public Currency Currency { get; init; }
        public InstrumentType InstrumentType { get; init; }
        public string IconUrl { get; set; }


        public Paper(string figi, string name, string ticker,
            Currency currency, InstrumentType instrumentType, string iconUrl)
        {
            Figi = figi;
            Name = name;
            Ticker = ticker;
            Currency = currency;
            InstrumentType = instrumentType;
            IconUrl = iconUrl;
        }

        public override string? ToString()
        {
            return Name;
        }
    }
}
