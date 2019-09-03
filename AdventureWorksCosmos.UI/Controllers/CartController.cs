using System;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Models.Cart;
using AdventureWorksCosmos.Products.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorksCosmos.UI
{
    public class CartController : Controller
    {
        private readonly AdventureWorks2016Context _db;
        private readonly IMediator _mediator;

        public CartController(AdventureWorks2016Context db, IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }

        public async Task<IActionResult> AddItem(int id)
        {
            var product = await _db.Product.SingleAsync(p => p.ProductId == id);
            var cart = GetCart();

            cart.IncrementQuantity(product);

            HttpContext.Session.Set("Cart", cart);

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();

            var request = new Checkout.Request {Cart = cart};

            var response = await _mediator.Send(request);

            HttpContext.Session.Set("Cart", cart);

            return RedirectToPage("/Orders/Show", new {id = response.OrderId});
        }

        private ShoppingCart GetCart() 
            => HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
    }
} 