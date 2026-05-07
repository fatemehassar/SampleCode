namespace Switch.Api.ExceptionHandeling
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex,
                    "Business Exception");

                context.Response.StatusCode = 400;

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        error = ex.Message
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled Exception");

                context.Response.StatusCode = 500;

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        error = "Internal Server Error"
                    });
            }
        }
    }
}
