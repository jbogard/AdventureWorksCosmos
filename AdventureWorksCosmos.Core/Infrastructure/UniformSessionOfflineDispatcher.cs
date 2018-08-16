using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Commands;
using NServiceBus.UniformSession;

namespace AdventureWorksCosmos.Core.Infrastructure
{
    public class UniformSessionOfflineDispatcher 
        : IOfflineDispatcher
    {
        private readonly IUniformSession _uniformSession;

        public UniformSessionOfflineDispatcher(IUniformSession uniformSession)
            => _uniformSession = uniformSession;

        public async Task DispatchOffline(DocumentBase document)
            => await _uniformSession.Send(ProcessDocumentMessages.New(document));
    }
}