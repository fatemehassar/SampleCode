using System.Threading.Channels;

namespace Topup.Infrastructure.Queue
{
    public class InMemoryQueue<T>
    {
        private readonly Channel<T> _channel =
            Channel.CreateUnbounded<T>();

        public async Task PublishAsync(T message)
        {
            await _channel.Writer.WriteAsync(message);
        }

        public IAsyncEnumerable<T> SubscribeAsync()
        {
            return _channel.Reader.ReadAllAsync();
        }
    }
}