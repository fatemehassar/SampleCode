using Polly;
using Polly.Wrap;

namespace Topup.Infrastructure.ExternalServices
{
    public class MciClient
    {
        private readonly AsyncPolicyWrap<bool> _policy;

        public MciClient()
        {
            var retry = Policy<bool>
                .Handle<Exception>()
                .OrResult(x => x == false)
                .WaitAndRetryAsync(
                    3,
                    retryAttempt =>
                        TimeSpan.FromSeconds(
                            Math.Pow(2, retryAttempt)));

            var breaker = Policy<bool>
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    5,
                    TimeSpan.FromSeconds(30));

            _policy = Policy.WrapAsync(retry, breaker);
        }

        public async Task<bool> TopupAsync(
            string mobileNo,
            decimal amount)
        {
            return await _policy.ExecuteAsync(async () =>
            {
                await Task.Delay(500);

                var success =
                    Random.Shared.Next(0, 3) != 0;

                return success;
            });
        }
    }
}