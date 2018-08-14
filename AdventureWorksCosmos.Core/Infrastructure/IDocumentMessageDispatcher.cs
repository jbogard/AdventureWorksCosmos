using System;
using System.Threading.Tasks;

namespace AdventureWorksCosmos.Core.Infrastructure
{
    public interface IDocumentMessageDispatcher
    {
        Task<Exception> Dispatch(DocumentBase document);
    }
}