using System;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Orders;

namespace AdventureWorksCosmos.Core.Models.Fulfillments
{
    public class OrderCreatedHandler : IDocumentMessageHandler<OrderCreated>
    {
        private readonly IDocumentDbRepository<OrderFulfillment> _repository;

        public OrderCreatedHandler(IDocumentDbRepository<OrderFulfillment> repository) 
            => _repository = repository;

        public async Task Handle(OrderCreated message)
        {
            var orderFulfillment = await FindOrCreateOrderFulfillment(message);

            orderFulfillment.Handle(message);

            await _repository.UpdateItemAsync(orderFulfillment);
        }

        private async Task<OrderFulfillment> FindOrCreateOrderFulfillment(OrderCreated message)
        {
            var orderFulfillment = (await _repository
                    .GetItemsAsync(s => s.OrderId == message.OrderId))
                .FirstOrDefault();

            if (orderFulfillment == null)
            {
                orderFulfillment = new OrderFulfillment
                {
                    Id = Guid.NewGuid(),
                    OrderId = message.OrderId
                };

                await _repository.CreateItemAsync(orderFulfillment);
            }

            return orderFulfillment;
        }
    }
}