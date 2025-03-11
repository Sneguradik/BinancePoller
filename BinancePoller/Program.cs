using BinancePoller;
using BinancePoller.Jobs;
using BinancePoller.Models;
using BinancePoller.Services;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddQuartz(options =>
{
    var jobKey = JobKey.Create(nameof(BinancePollerJob));
    options.AddJob<BinancePollerJob>(jobKey)
        .AddTrigger(trigger => trigger.ForJob(jobKey).WithCronSchedule("0 1 * * * *"));
});
builder.Services.AddQuartzHostedService();

builder.Services.AddSingleton<IStateService, StateService>();
builder.Services.AddHttpClient<IBinanceService,BinanceService>(opt=>
    opt.BaseAddress = new Uri("https://api.binance.com"));
builder.Services.AddHttpClient<IIndexService,IndexService>(opt => opt.BaseAddress = new Uri("https://indexapi.spbexchange.ru"));
builder.Services.Configure<List<FetchItem>>(builder.Configuration.GetSection("FetchConfig"));

var host = builder.Build();
host.Run(); 