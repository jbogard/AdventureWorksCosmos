using System;
using System.Collections.Generic;
using System.Linq;
using AdventureWorksCosmos.Core.Models.Inventory;
using AdventureWorksCosmos.Core.Models.Orders;

namespace AdventureWorksCosmos.Core.Models.Fulfillments
{
    public class OrderFulfillment : DocumentBase
    {
        public Guid OrderId { get; set; }
        public bool IsCancelled { get; set; }
        public bool CancelOrderRequested { get; set; }
        public List<LineItem> LineItems { get; set; }
        public bool OrderApproved { get; set; }
        public bool OrderRejected { get; set; }

        public class LineItem
        {
            public int ProductId { get; set; }
            public int AmountRequested { get; set; }
            public bool StockConfirmed { get; set; }
            public bool StockReturnRequested { get; set; }
        }
        
        public void Handle(OrderCreated message)
        {
            Process(message, m =>
            {
                if (IsCancelled)
                    return;

                LineItems = m.LineItems
                    .Select(li => new LineItem
                    {
                        ProductId = li.ProductId,
                        AmountRequested = li.Quantity
                    })
                    .ToList();

                foreach (var lineItem in LineItems)
                {
                    Send(new StockRequest
                    {
                        Id = Guid.NewGuid(),
                        ProductId = lineItem.ProductId,
                        AmountRequested = lineItem.AmountRequested,
                        OrderFulfillmentId = Id
                    });
                }
            });
        }

        public void Handle(OrderApproved message)
        {
            Process(message, m =>
            {
                if (IsCancelled)
                    return;

                OrderApproved = true;
            });
        }

        public void Handle(StockRequestConfirmed message)
        {
            Process(message, m =>
            {
                var lineItem = LineItems.Single(li => li.ProductId == m.ProductId);
                lineItem.StockConfirmed = true;

                if (IsCancelled && !lineItem.StockReturnRequested)
                {
                    ReturnStock(lineItem);
                    return;
                }

                if (LineItems.All(li => li.StockConfirmed))
                {
                    Send(new OrderFulfillmentSuccessful
                    {
                        Id = Guid.NewGuid(),
                        OrderId = OrderId
                    });
                }
            });
        }

        public void Handle(StockRequestDenied message)
        {
            Process(message, m =>
            {
                Cancel();
            });
        }

        public void Handle(OrderRejected message)
        {
            Process(message, m =>
            {
                OrderRejected = true;

                Cancel();
            });
        }

        private void Cancel()
        {
            IsCancelled = true;

            if (!CancelOrderRequested && !OrderRejected)
            {
                CancelOrderRequested = true;
                Send(new CancelOrderRequest
                {
                    Id = Guid.NewGuid(),
                    OrderId = OrderId
                });
            }

            foreach (var lineItem in LineItems.Where(li => li.StockConfirmed))
            {
                ReturnStock(lineItem);
            }
        }


        private void ReturnStock(LineItem lineItem)
        {
            if (lineItem.StockReturnRequested)
                return;

            lineItem.StockReturnRequested = true;
            Send(new StockReturnRequested
            {
                Id = Guid.NewGuid(),
                ProductId = lineItem.ProductId,
                AmountToReturn = lineItem.AmountRequested
            });
        }
    }
}