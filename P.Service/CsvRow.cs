using CsvHelper.Configuration.Attributes;

namespace P.Service
{
    public class CsvRow
    {
        [Name("Local Time")]
        public string LocalTime { get; set; }

        public double Volume { get; set; }
    }
}
