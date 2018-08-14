using System.Threading;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using MediatR;

namespace AdventureWorksCosmos.Core.Models.Orders
{
    public class ApproveOrderHandler : IRequestHandler<ApproveOrderRequest>
    {
        private readonly IDocumentDBRepository<OrderRequest> _orderRepository;

        public ApproveOrderHandler(IDocumentDBRepository<OrderRequest> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(ApproveOrderRequest request, CancellationToken cancellationToken)
        {
            var orderRequest = await _orderRepository.GetItemAsync(request.Id);

            orderRequest.Approve();

            await _orderRepository.UpdateItemAsync(orderRequest);

            return Unit.Value;
        }
    }
}