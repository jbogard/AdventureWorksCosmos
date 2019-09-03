using System;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core.Commands;

namespace AdventureWorksCosmos.Core.Infrastructure
{
    public interface IDocumentMessageDispatcher
    {
        Task<Exception> Dispatch(
            DocumentBase document);


        Task Dispatch(ProcessDocumentMessages command);
    }
}