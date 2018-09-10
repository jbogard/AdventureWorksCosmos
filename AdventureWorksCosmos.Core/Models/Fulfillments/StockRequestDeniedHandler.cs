using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Inventory;

namespace AdventureWorksCosmos.Core.Models.Fulfillments
{
    public class StockRequestDeniedHandler : IDocumentMessageHandler<StockRequestDenied>
    {
        private readonly IDocumentDBRepository<OrderFulfillment> _repository;

        public StockRequestDeniedHandler(IDocumentDBRepository<OrderFulfillment> repository)
            => _repository = repository;

        public async Task Handle(StockRequestDenied message)
        {
            var orderFulfillment = await _repository.GetItemAsync(message.OrderFulfillmentId);

            orderFulfillment.Handle(message);

            await _repository.UpdateItemAsync(orderFulfillment);

        }
    }
}