using System;
using System.Collections.Generic;
using System.Linq;
using AdventureWorksCosmos.Core.Models.Cart;

namespace AdventureWorksCosmos.Core.Models.Orders
{
    public enum Status
    {
        New = 0,
        Submitted = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public class OrderRequest : DocumentBase
    {
        public OrderRequest() { }

        public OrderRequest(ShoppingCart cart)
        {
            Id = Guid.NewGuid();
            Customer = new Customer
            {
                FirstName = "Jane",
                MiddleName = "Mary",
                LastName = "Doe"
            };
            Items = cart.Items.Select(li => new LineItem
            {
                ProductId = li.Key,
                Quantity = li.Value.Quantity,
                ListPrice = li.Value.ListPrice,
                ProductName = li.Value.ProductName
            }).ToList();
            Status = Status.New;

            Send(new OrderCreated
            {
                Id = Guid.NewGuid(),
                OrderId = Id,
                LineItems = Items
                    .Select(item => new OrderCreated.LineItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    })
                    .ToList()
            });
        }

        public List<LineItem> Items { get; set; }

        public Status Status { get; set; }

        public decimal Total
        {
            get { return Items.Sum(li => li.Subtotal); }
        }

        public Customer Customer { get; set; }

        public void Approve()
        {
            if (Status == Status.Approved)
                return;

            if (Status == Status.Rejected)
                throw new InvalidOperationException("Cannot approve a rejected order.");

            Status = Status.Approved;
            Send(new OrderApproved
            {
                Id = Guid.NewGuid(),
                OrderId = Id
            });
        }

        public void Reject()
        {
            if (Status == Status.Rejected)
                return;

            if (Status == Status.Approved)
                throw new InvalidOperationException("Cannot reject an approved order.");

            Status = Status.Rejected;
            Send(new OrderRejected
            {
                Id = Guid.NewGuid(),
                OrderId = Id
            });
        }

        public void Handle(CancelOrderRequest message)
        {
            Process(message, m =>
            {
                if (Status == Status.Rejected)
                    return;

                Status = Status.Cancelled;
            });
        }
    }

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
    }

    public class LineItem
    {
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Subtotal => Quantity * ListPrice;
    }
}