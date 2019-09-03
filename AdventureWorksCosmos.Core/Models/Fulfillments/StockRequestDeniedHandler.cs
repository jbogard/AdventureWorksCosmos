using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Inventory;

namespace AdventureWorksCosmos.Core.Models.Fulfillments
{
    public class StockRequestDeniedHandler : IDocumentMessageHandler<StockRequestDenied>
    {
        private readonly IDocumentDbRepository<OrderFulfillment> _repository;

        public StockRequestDeniedHandler(IDocumentDbRepository<OrderFulfillment> repository)
            => _repository = repository;

        public async Task Handle(StockRequestDenied message)
        {
            var orderFulfillment = await _repository.GetItemAsync(message.OrderFulfillmentId);

            orderFulfillment.Handle(message);

            await _repository.UpdateItemAsync(orderFulfillment);

        }
    }
}