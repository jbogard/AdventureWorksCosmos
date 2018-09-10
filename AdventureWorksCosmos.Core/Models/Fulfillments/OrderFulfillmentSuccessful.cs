using System;

namespace AdventureWorksCosmos.Core.Models.Fulfillments
{
    public class OrderFulfillmentSuccessful : IDocumentMessage
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

    }
}