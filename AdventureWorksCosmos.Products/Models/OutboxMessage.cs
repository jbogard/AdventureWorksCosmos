using System;

namespace AdventureWorksCosmos.Products.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Body { get; set; }
        public DateTime? DispatchedAt { get; set; }
    }
}