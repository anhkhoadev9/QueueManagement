using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            
            _logger.LogInformation("CQRS Request Started: {Name} {@Request}", requestName, request);
            
            var timer = new Stopwatch();
            timer.Start();

            var response = await next();

            timer.Stop();
            
            if (timer.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning("CQRS Request Long Running: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}", requestName, timer.ElapsedMilliseconds, request);
            }

            _logger.LogInformation("CQRS Request Completed: {Name} {@Response}", requestName, response);

            return response;
        }
    }
}
