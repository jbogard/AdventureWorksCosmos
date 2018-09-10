using System;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;

namespace AdventureWorksCosmos.Core.Models.Inventory
{
    public class StockReturnRequestedHandler : IDocumentMessageHandler<StockReturnRequested>
    {
        private readonly IDocumentDBRepository<Stock> _repository;

        public StockReturnRequestedHandler(IDocumentDBRepository<Stock> repository)
            => _repository = repository;

        public async Task Handle(StockReturnRequested message)
        {
            var stock = (await _repository
                    .GetItemsAsync(s => s.ProductId == message.ProductId))
                .FirstOrDefault();

            if (stock == null)
            {
                stock = new Stock
                {
                    Id = Guid.NewGuid(),
                    ProductId = message.ProductId,
                    QuantityAvailable = 100
                };

                await _repository.CreateItemAsync(stock);
            }

            stock.Handle(message);

            await _repository.UpdateItemAsync(stock);
        }
    }
}