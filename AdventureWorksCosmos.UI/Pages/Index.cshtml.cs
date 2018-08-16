using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NServiceBus;

namespace AdventureWorksCosmos.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMessageSession _messageSession;

        public IndexModel(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public Task OnGet()
        {
            return _messageSession.Send(new SaySomething {Message = "Hello!!"});
        }
    }
}
