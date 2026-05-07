using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>  : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<
            LoggingBehavior<TRequest, TResponse>>
            _logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Handling {Request}",
                typeof(TRequest).Name);

            return await next();
        }
    }
}
