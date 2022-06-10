using PeriodicBackgroundTaskSample;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<SampleService>();

// Register as singleton first so it can be injected through Dependency Injection
builder.Services.AddSingleton<PeriodicHostedService>();

// Add as hosted service using the instance registered as singleton before
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<PeriodicHostedService>());

WebApplication app = builder.Build();

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
