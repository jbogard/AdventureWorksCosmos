using System;

namespace AdventureWorksCosmos.Core.Models.Inventory
{
    public class StockRequest : IDocumentMessage
    {
        public Guid Id { get; set; }
        public int ProductId { get; set; }
        public int AmountRequested { get; set; }
        public Guid OrderFulfillmentId { get; set; }
    }
}