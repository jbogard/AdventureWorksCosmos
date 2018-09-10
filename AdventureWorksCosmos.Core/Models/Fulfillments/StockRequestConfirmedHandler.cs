using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Inventory;

namespace AdventureWorksCosmos.Core.Models.Fulfillments
{
    public class StockRequestConfirmedHandler : IDocumentMessageHandler<StockRequestConfirmed>
    {
        private readonly IDocumentDBRepository<OrderFulfillment> _repository;

        public StockRequestConfirmedHandler(IDocumentDBRepository<OrderFulfillment> repository) 
            => _repository = repository;

        public async Task Handle(StockRequestConfirmed message)
        {
            var orderFulfillment = await _repository.GetItemAsync(message.OrderFulfillmentId);

            orderFulfillment.Handle(message);

            await _repository.UpdateItemAsync(orderFulfillment);

        }
    }
}