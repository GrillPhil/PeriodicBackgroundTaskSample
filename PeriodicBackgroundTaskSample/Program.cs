using PeriodicBackgroundTaskSample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<SampleService>();
builder.Services.AddSingleton<PeriodicHostedService>();
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<PeriodicHostedService>());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/background", (
    PeriodicHostedService service) =>
{
    return new PeriodicHostedServiceState(service.IsEnabled);
});
app.MapMethods("/background", new[] { "PATCH" }, (
    PeriodicHostedServiceState state, 
    PeriodicHostedService service) =>
{
    service.IsEnabled = state.IsEnabled;
});

app.Run();
