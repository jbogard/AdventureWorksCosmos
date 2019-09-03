using System;

namespace AdventureWorksCosmos.Core.Models.Inventory
{
    public class Stock : DocumentBase
    {
        public int QuantityAvailable { get; set; }

        public int ProductId { get; set; }

        public void Handle(StockRequest message)
        {
            Receive(message, e =>
            {
                if (QuantityAvailable >= message.AmountRequested)
                {
                    QuantityAvailable -= e.AmountRequested;
                    Send(new StockRequestConfirmed
                    {
                        Id = Guid.NewGuid(),
                        OrderFulfillmentId = e.OrderFulfillmentId,
                        ProductId = ProductId
                    });
                }
                else
                {
                    Send(new StockRequestDenied
                    {
                        Id = Guid.NewGuid(),
                        OrderFulfillmentId = e.OrderFulfillmentId,
                        ProductId = ProductId
                    });
                }
            });
        }

        public void Handle(StockReturnRequested message) 
            => Receive(message, e => QuantityAvailable += e.AmountToReturn);
    }
}