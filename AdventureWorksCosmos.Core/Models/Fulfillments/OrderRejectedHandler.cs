using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Orders;

namespace AdventureWorksCosmos.Core.Models.Fulfillments
{
    public class OrderRejectedHandler : IDocumentMessageHandler<OrderRejected>
    {
        private readonly IDocumentDBRepository<OrderFulfillment> _repository;

        public OrderRejectedHandler(IDocumentDBRepository<OrderFulfillment> repository)
            => _repository = repository;

        public async Task Handle(OrderRejected message)
        {
            var orderFulfillment = (await _repository
                    .GetItemsAsync(s => s.OrderId == message.OrderId))
                .FirstOrDefault();

            if (orderFulfillment == null)
            {
                orderFulfillment = new OrderFulfillment
                {
                    OrderId = message.OrderId
                };

                await _repository.CreateItemAsync(orderFulfillment);
            }

            orderFulfillment.Handle(message);

            await _repository.UpdateItemAsync(orderFulfillment);

        }
    }
}