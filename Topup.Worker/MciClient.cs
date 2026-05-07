using Polly;
using Polly.Wrap;

public class MciClient
{
    private readonly AsyncPolicyWrap _policy;

    public MciClient()
    {
        var retry = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * i));

        var breaker = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));

        _policy = Policy.WrapAsync(retry, breaker);
    }

    public async Task<bool> CallAsync()
    {
        return await _policy.ExecuteAsync(async () =>
        {
            await Task.Delay(100);

            if (Random.Shared.Next(0, 4) == 0)
                throw new Exception("MCI Fail");

            return true;
        });
    }
}