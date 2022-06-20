using CsvHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P.Service.Config;
using System.Globalization;

namespace P.Service
{
    public class PowerAggregatePrinter
    {
        #region Fields
        private readonly TimeOnly _start = TimeOnly.Parse("23:00");
        private readonly IOptions<ReportConfig> _config;
        private readonly ILogger<PowerAggregatePrinter> _logger;
        #endregion Fields

        #region Properties
        public string FileName { get; private set; }

        public IEnumerable<CsvRow> Records { get; private set; }
        #endregion Properties

        #region Constructor
        public PowerAggregatePrinter(IOptions<ReportConfig> config_, ILogger<PowerAggregatePrinter> logger_)
        {
            _config = config_;
            _logger = logger_;
        }
        #endregion Constructor

        #region Methods
        public async Task PrintAsync(PowerAggregator aggr_, CancellationToken stoppingToken_)
        {
            Records = GetRecords(aggr_.TotalVolumes);

            FileName = GetFilePath(aggr_.Timestamp);
            _logger.LogInformation("Write to: " + FileName);

            using (StreamWriter writer = new StreamWriter(FileName))
            {
                using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    await csv.WriteRecordsAsync(Records, stoppingToken_);
                }
            }
        }

        private IEnumerable<CsvRow> GetRecords(double[] volumes_)
        {
            for(int i=0; i<volumes_.Length; i++)
            {
                yield return new CsvRow
                {
                    LocalTime = _start.AddHours(i).ToString("HH:mm"),
                    Volume = volumes_[i]
                };
            }
        }

        private string GetFilePath(DateTime timestamp)
        {
            string path = _config.Value?.Path;
            string filename = string.Format("PowerPosition_{0}.csv", timestamp.ToString("yyyyMMdd_HHmm"));
            if (string.IsNullOrEmpty(path))
            {
                return filename;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, filename);
        }
        #endregion Methods
    }
}