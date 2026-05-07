using Topup.Application.Interfaces;

namespace Topup.Infrastructure.Services
{
    public class SwitchService : ISwitchService
    {
        public async Task AdviceAsync(Guid txId)
        {
            await Task.Delay(100);

            Console.WriteLine(
                $"ADVICE SUCCESS => TX:{txId}");
        }

        public async Task ReverseAsync(Guid txId)
        {
            await Task.Delay(100);

            Console.WriteLine(
                $"REVERSE EXECUTED => TX:{txId}");
        }
    }
}
