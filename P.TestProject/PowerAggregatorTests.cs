using P.Service;
using Services;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Xunit;
using Moq;

namespace P.TestProject
{
    public class PowerAggregatorTests
    {
        private Microsoft.Extensions.Options.IOptions<Service.Config.ReportConfig> _reportConfig;
        private Microsoft.Extensions.Logging.ILogger<PowerAggregatePrinter> _logger;
        private CancellationToken _cancellationToken;
        public PowerAggregatorTests()
        {
            _reportConfig = new Mock<Microsoft.Extensions.Options.IOptions<Service.Config.ReportConfig>>().Object;
            _logger = new Mock<Microsoft.Extensions.Logging.ILogger<PowerAggregatePrinter>>().Object;
            _cancellationToken = new CancellationToken();
        }

        [Fact]
        public void TestAggregator()
        {
            DateTime date = new DateTime(2022, 6, 19, 13, 5, 0);

            PowerTrade firstTrade = PowerTrade.Create(date, 24);
            foreach (PowerPeriod period in firstTrade.Periods)
            {
                period.Volume = 100;
            }

            PowerTrade secondTrade = PowerTrade.Create(date, 24);
            for (int i = 0; i < 12; i++)
            {
                secondTrade.Periods[i].Volume = 50;
            }
            for (int i = 12; i < 24; i++)
            {
                secondTrade.Periods[i].Volume = -20;
            }

            PowerAggregator aggr = new PowerAggregator(date);
            aggr.Aggregate(new List<PowerTrade>() { firstTrade, secondTrade});
            double[] actual = aggr.TotalVolumes;

            double[] expected = new double[24];
            for (int i = 0; i < 12; i++)
            {
                expected[i] = 150;
            }
            for (int i = 12; i < 24; i++)
            {
                expected[i] = 80;
            }

            Assert.Equal<double>(expected, actual);
        }

        [Fact]
        public void TestPrinter()
        {
            DateTime date = new DateTime(2022, 6, 19, 13, 5, 0);

            PowerTrade firstTrade = PowerTrade.Create(date, 24);
            foreach (PowerPeriod period in firstTrade.Periods)
            {
                period.Volume = 100;
            }

            PowerTrade secondTrade = PowerTrade.Create(date, 24);
            for (int i = 0; i < 12; i++)
            {
                secondTrade.Periods[i].Volume = 50;
            }
            for (int i = 12; i < 24; i++)
            {
                secondTrade.Periods[i].Volume = -20;
            }

            PowerAggregator aggr = new PowerAggregator(date);
            aggr.Aggregate(new List<PowerTrade>() { firstTrade, secondTrade });

            PowerAggregatePrinter printer = new PowerAggregatePrinter(_reportConfig, _logger);
            printer.PrintAsync(aggr, _cancellationToken).Wait();

            string actualFileName = printer.FileName;
            string expectedFileName = "PowerPosition_20220619_1305.csv";
            Assert.Equal(expectedFileName, actualFileName);

            List<CsvRow> actualRecords = printer.Records.ToList();
            List<CsvRow> expectedRecords = new List<CsvRow>()
            {
                new CsvRow{ LocalTime = "23:00", Volume = 150 },
                new CsvRow{ LocalTime = "00:00", Volume = 150 },
                new CsvRow{ LocalTime = "01:00", Volume = 150 },
                new CsvRow{ LocalTime = "02:00", Volume = 150 },
                new CsvRow{ LocalTime = "03:00", Volume = 150 },
                new CsvRow{ LocalTime = "04:00", Volume = 150 },
                new CsvRow{ LocalTime = "05:00", Volume = 150 },
                new CsvRow{ LocalTime = "06:00", Volume = 150 },
                new CsvRow{ LocalTime = "07:00", Volume = 150 },
                new CsvRow{ LocalTime = "08:00", Volume = 150 },
                new CsvRow{ LocalTime = "09:00", Volume = 150 },
                new CsvRow{ LocalTime = "10:00", Volume = 150 },
                new CsvRow{ LocalTime = "11:00", Volume = 80 },
                new CsvRow{ LocalTime = "12:00", Volume = 80 },
                new CsvRow{ LocalTime = "13:00", Volume = 80 },
                new CsvRow{ LocalTime = "14:00", Volume = 80 },
                new CsvRow{ LocalTime = "15:00", Volume = 80 },
                new CsvRow{ LocalTime = "16:00", Volume = 80 },
                new CsvRow{ LocalTime = "17:00", Volume = 80 },
                new CsvRow{ LocalTime = "18:00", Volume = 80 },
                new CsvRow{ LocalTime = "19:00", Volume = 80 },
                new CsvRow{ LocalTime = "20:00", Volume = 80 },
                new CsvRow{ LocalTime = "21:00", Volume = 80 },
                new CsvRow{ LocalTime = "22:00", Volume = 80 }
            };
            for(int i=0; i<24; i++)
            {
                Assert.Equal(expectedRecords[i].LocalTime, actualRecords[i].LocalTime);
                Assert.Equal(expectedRecords[i].Volume, actualRecords[i].Volume);
            }
        }
    }
}