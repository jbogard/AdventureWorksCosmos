using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace AdventureWorksCosmos.Dispatcher
{
    public class SaySomethingHandler : IHandleMessages<SaySomething>
    {
        static ILog log = LogManager.GetLogger<SaySomethingHandler>();

        public Task Handle(SaySomething message, IMessageHandlerContext context)
        {
            log.Info($"Message: {message.Message}");

            return Task.CompletedTask;
        }
    }
}