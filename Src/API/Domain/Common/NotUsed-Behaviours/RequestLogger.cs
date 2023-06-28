//using Common.Interfaces;
//using MediatR.Pipeline;
//using Microsoft.Extensions.Logging;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Common.Behaviours
//{
//    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest>
//    {
//        private readonly ILogger _logger;
//        //private readonly ICurrentUserService _currentUserService;

//        public RequestLogger(ILogger<TRequest> logger) //, ICurrentUserService currentUserService)
//        {
//            _logger = logger;
//            //_currentUserService = currentUserService;
//        }

//        public Task Process(TRequest request, CancellationToken cancellationToken)
//        {
//            var name = typeof(TRequest).Name;

//            //_logger.LogInformation("FMMediatr Request: {Name} {@UserId} {@Request}",
//            //    name, _currentUserService.UserId, request);
//            _logger.LogInformation("FMMediatr Request: {Name} {@Request}",
//                name, request);

//            return Task.CompletedTask;
//        }
//    }
//}
