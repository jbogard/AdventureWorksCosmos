using System.Threading.Tasks;

namespace AdventureWorksCosmos.Core.Infrastructure
{
    public interface IOfflineDispatcher
    {
        Task DispatchOffline(DocumentBase document);
    }
}