namespace PeriodicBackgroundTaskSample;

class SampleService
{
    private readonly ILogger<SampleService> _logger;

    public SampleService(ILogger<SampleService> logger)
    {
        _logger = logger;
    }

    public async Task DoSomethingAsync()
    {
        await Task.Delay(100);
        _logger.LogInformation(
            "Sample Service did something.");
    }
}
