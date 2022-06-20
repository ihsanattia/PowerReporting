using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P.Service;
using P.Service.Config;
using Services;

public sealed class PowerAggregatorService : BackgroundService
{
    #region Fields
    private readonly static TimeSpan DefaultTimeSpan = TimeSpan.FromHours(1);
    private readonly IPowerService _powerService;
    private readonly ILogger<PowerAggregatorService> _logger;
    private readonly PowerAggregatePrinter _printer;
    private readonly IOptions<ServiceConfig> _config;
    #endregion Fields

    #region Constructors
    public PowerAggregatorService(IPowerService powerService_, PowerAggregatePrinter printer_, IOptions<ServiceConfig> config_, ILogger<PowerAggregatorService> logger_)
    {
        _powerService = powerService_;
        _printer = printer_;
        _config = config_;
        _logger = logger_;
    }
    #endregion Constructors

    #region Methods
    protected override async Task ExecuteAsync(CancellationToken stoppingToken_)
    {
        _logger.LogInformation("Started");

        while (!stoppingToken_.IsCancellationRequested)
        {
            _logger.LogInformation("Execute");
            _ = AggregateAsync(stoppingToken_).ConfigureAwait(false);

            TimeSpan delay = _config.Value.Delay ?? DefaultTimeSpan;
            _logger.LogInformation($"Wait {delay} seconds");
            await Task.Delay(delay, stoppingToken_);
        }

        _logger.LogInformation("Completed");
    }

    private async Task AggregateAsync(CancellationToken stoppingToken_)
    {
        IEnumerable<PowerTrade> trades = await _powerService.GetTradesAsync(DateTime.Now).ConfigureAwait(false);
        PowerAggregator aggr = new PowerAggregator(trades.First().Date);
        aggr.Aggregate(trades);
        _logger.LogInformation($"{trades.Count()} trades recieved");
        await _printer.PrintAsync(aggr, stoppingToken_);
    }
    #endregion Methods
}