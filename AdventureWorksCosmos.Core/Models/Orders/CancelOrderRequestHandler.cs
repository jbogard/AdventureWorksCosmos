using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;

namespace AdventureWorksCosmos.Core.Models.Orders
{
    public class CancelOrderRequestHandler : IDocumentMessageHandler<CancelOrderRequest>
    {
        private readonly IDocumentDbRepository<OrderRequest> _repository;

        public CancelOrderRequestHandler(IDocumentDbRepository<OrderRequest> repository) 
            => _repository = repository;

        public async Task Handle(CancelOrderRequest message)
        {
            var order = await _repository.GetItemAsync(message.OrderId);

            order.Handle(message);

            await _repository.UpdateItemAsync(order);
        }
    }
}