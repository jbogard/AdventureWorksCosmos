using System;
using System.Threading;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using MediatR;

namespace AdventureWorksCosmos.Core.Models.Orders
{
    public class RejectOrder
    {
        public class Request : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IDocumentDBRepository<OrderRequest> _orderRepository;

            public Handler(IDocumentDBRepository<OrderRequest> orderRepository)
            {
                _orderRepository = orderRepository;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var orderRequest = await _orderRepository.GetItemAsync(request.Id);

                orderRequest.Reject();
                
                await _orderRepository.UpdateItemAsync(orderRequest);

                return Unit.Value;
            }
        }
    }
}