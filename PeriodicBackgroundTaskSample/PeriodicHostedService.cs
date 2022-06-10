namespace PeriodicBackgroundTaskSample;

class PeriodicHostedService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly ILogger<PeriodicHostedService> _logger;
    private readonly IServiceScopeFactory _factory;
    private int _executionCount = 0;
    public bool IsEnabled { get; set; }

    public PeriodicHostedService(
        ILogger<PeriodicHostedService> logger, 
        IServiceScopeFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Use PeriodicTimer to prevent blocking of resources
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            // Repeat while Hosted Service is not stopped
            !stoppingToken.IsCancellationRequested &&
            // Wait for the timer to tick but as long as Hosted Service is not stopped
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (IsEnabled)
                {
                    // Create a scope
                    await using AsyncServiceScope asyncScope = _factory.CreateAsyncScope();
                    // Get service from scope
                    SampleService sampleService = asyncScope.ServiceProvider.GetRequiredService<SampleService>();
                    await sampleService.DoSomethingAsync();
                    _executionCount++;
                    _logger.LogInformation(
                        $"Executed PeriodicHostedService - Count: {_executionCount}");
                }
                else
                {
                    _logger.LogInformation(
                        "Skipped PeriodicHostedService");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed to execute PeriodicHostedService with exception message {ex.Message}. Good luck next round!");
            }
        }
    }
}
