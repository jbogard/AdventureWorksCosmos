using System;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Orders;

namespace AdventureWorksCosmos.Core.Models.Inventory
{
    public class UpdateStockFromItemPurchasedHandler : IDocumentMessageHandler<ItemPurchased>
    {
        private readonly IDocumentDBRepository<Stock> _repository;

        public UpdateStockFromItemPurchasedHandler(IDocumentDBRepository<Stock> repository) 
            => _repository = repository;

        public async Task Handle(ItemPurchased message)
        {
            await Task.CompletedTask;
            throw new Exception("Blammo");
            //var stock = (await _repository
            //    .GetItemsAsync(s => s.ProductId == message.ProductId))
            //    .FirstOrDefault();

            //if (stock == null)
            //{
            //    stock = new Stock
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductId = message.ProductId,
            //        QuantityAvailable = 100
            //    };

            //    await _repository.CreateItemAsync(stock);
            //}

            //stock.Handle(message);

            //await _repository.UpdateItemAsync(stock);
        }
    }
}