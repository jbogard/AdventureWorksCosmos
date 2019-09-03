using System;
using System.Threading;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Orders;
using MediatR;

namespace AdventureWorksCosmos.Core.Models.Cart
{
    public class Checkout
    {
        public class Request : IRequest<Response>
        {
            public ShoppingCart Cart { get; set; }
        }

        public class Response
        {
            public Guid OrderId { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IDocumentDbRepository<OrderRequest> _repository;

            public Handler(IDocumentDbRepository<OrderRequest> repository) 
                => _repository = repository;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var orderRequest = new OrderRequest(request.Cart);

                await _repository.CreateItemAsync(orderRequest);

                request.Cart.Items.Clear();

                return new Response {OrderId = orderRequest.Id};
            }
        }
    }
}