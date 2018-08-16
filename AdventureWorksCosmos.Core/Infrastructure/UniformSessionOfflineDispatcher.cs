using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Commands;
using NServiceBus.UniformSession;

namespace AdventureWorksCosmos.Core.Infrastructure
{
    public class UniformSessionOfflineDispatcher : IOfflineDispatcher
    {
        private readonly IUniformSession _uniformSession;

        public UniformSessionOfflineDispatcher(IUniformSession uniformSession)
            => _uniformSession = uniformSession;

        public Task DispatchOffline(DocumentBase document)
            => _uniformSession.Send(ProcessDocumentMessages.New(document));
    }
}