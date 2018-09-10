using System;

namespace AdventureWorksCosmos.Core.Models.Orders
{
    public class OrderRejected : IDocumentMessage
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
    }
}