using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Commands;
using AdventureWorksCosmos.Core.Infrastructure;
using NServiceBus;

namespace AdventureWorksCosmos.Dispatcher
{
    public class ProcessDocumentMessagesHandler
        : IHandleMessages<ProcessDocumentMessages>
    {
        private readonly IDocumentMessageDispatcher _dispatcher;

        public ProcessDocumentMessagesHandler(IDocumentMessageDispatcher dispatcher) 
            => _dispatcher = dispatcher;

        public Task Handle(ProcessDocumentMessages message, IMessageHandlerContext context) 
            => _dispatcher.Dispatch(message);
    }
}