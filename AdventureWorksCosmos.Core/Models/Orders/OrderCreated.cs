using System;
using System.Collections.Generic;

namespace AdventureWorksCosmos.Core.Models.Orders
{
    public class OrderCreated : IDocumentMessage
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        public List<LineItem> LineItems { get; set; }

        public class LineItem
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}