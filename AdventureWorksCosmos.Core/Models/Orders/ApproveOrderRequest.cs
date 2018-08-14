using System;
using MediatR;

namespace AdventureWorksCosmos.Core.Models.Orders
{
    public class ApproveOrderRequest : IRequest
    {
        public Guid Id { get; set; }
    }
}