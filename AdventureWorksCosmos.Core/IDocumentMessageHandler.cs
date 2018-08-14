using System.Threading.Tasks;

namespace AdventureWorksCosmos.Core
{
    public interface IDocumentMessageHandler<in T>
        where T : IDocumentMessage
    {
        Task Handle(T message);
    }
}