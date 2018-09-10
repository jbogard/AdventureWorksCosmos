using System;

namespace AdventureWorksCosmos.Core.Models.Inventory
{
    public class StockRequestConfirmed : IDocumentMessage
    {
        public Guid Id { get; set; }
        public Guid OrderFulfillmentId { get; set; }
        public int ProductId { get; set; }
    }
}