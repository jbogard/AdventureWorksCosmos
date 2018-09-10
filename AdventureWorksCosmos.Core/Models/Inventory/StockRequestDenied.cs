using System;

namespace AdventureWorksCosmos.Core.Models.Inventory
{
    public class StockRequestDenied : IDocumentMessage
    {
        public Guid Id { get; set; }
        public Guid OrderFulfillmentId { get; set; }
        public int ProductId { get; set; }
    }
}