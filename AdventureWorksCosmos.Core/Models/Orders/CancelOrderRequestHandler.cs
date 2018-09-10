using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;

namespace AdventureWorksCosmos.Core.Models.Orders
{
    public class CancelOrderRequestHandler : IDocumentMessageHandler<CancelOrderRequest>
    {
        private readonly IDocumentDBRepository<OrderRequest> _repository;

        public CancelOrderRequestHandler(IDocumentDBRepository<OrderRequest> repository) 
            => _repository = repository;

        public async Task Handle(CancelOrderRequest message)
        {
            var order = await _repository.GetItemAsync(message.OrderId);

            order.Handle(message);
        }
    }
}