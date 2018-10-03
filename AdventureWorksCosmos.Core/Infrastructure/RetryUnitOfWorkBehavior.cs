using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Documents;

namespace AdventureWorksCosmos.Core.Infrastructure
{
    public class RetryUnitOfWorkBehavior<TRequest, TResponse> 
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RetryUnitOfWorkBehavior(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork;

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var retryCount = 0;

            while (true)
            {
                try
                {
                    return next();
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != HttpStatusCode.PreconditionFailed)
                        throw;

                    if (retryCount >= 5)
                        throw;

                    _unitOfWork.Reset();

                    retryCount++;
                }
            }
        }
    }
}