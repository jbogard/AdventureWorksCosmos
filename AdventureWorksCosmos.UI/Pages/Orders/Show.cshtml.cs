using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdventureWorksCosmos.UI.Pages.Orders
{
    public class ShowModel : PageModel
    {
        private readonly IDocumentDbRepository<OrderRequest> _db;

        public ShowModel(IDocumentDbRepository<OrderRequest> db) => _db = db;

        public async Task OnGet(Guid id)
        {
            Order = await _db.GetItemAsync(id);
        }

        public OrderRequest Order { get; set; }
    }
}