using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using P.Service;
using Services;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = ".NET Power Service";
    })
    .ConfigureServices((builder, services)  =>
    {
        services.Configure<P.Service.Config.ServiceConfig>(builder.Configuration.GetSection("Service"));
        services.Configure<P.Service.Config.ReportConfig>(builder.Configuration.GetSection("Report"));
        services.AddTransient<PowerAggregatePrinter>();
        services.AddSingleton<IPowerService, PowerService>();
        services.AddHostedService<PowerAggregatorService>();
    })
    .Build();

await host.RunAsync();
