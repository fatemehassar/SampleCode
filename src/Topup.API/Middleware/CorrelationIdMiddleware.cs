namespace Topup.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderKey =
       "X-Correlation-ID";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context)
        {
            var correlationId =
                context.Request.Headers[HeaderKey]
                    .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(
                    correlationId))
            {
                correlationId =
                    Guid.NewGuid().ToString();
            }

            context.Items["CorrelationId"] =
                correlationId;

            context.Response.Headers[HeaderKey] =
                correlationId;

            using var scope =
                context.RequestServices
                    .GetRequiredService<
                        ILogger<CorrelationIdMiddleware>>()
                    .BeginScope(new Dictionary<string, object>
                    {
                        ["CorrelationId"] = correlationId
                    });

            await _next(context);
        }
    }
}
