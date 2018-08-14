using System;

namespace AdventureWorksCosmos.Core
{
    public interface IDocumentMessage
    {
        Guid Id { get; }
    }
}