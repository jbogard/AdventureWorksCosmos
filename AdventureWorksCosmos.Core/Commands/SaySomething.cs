using NServiceBus;

namespace AdventureWorksCosmos.Core.Commands
{
    public class SaySomething : ICommand
    {
        public string Message { get; set; }
    }
}