using System;
using NServiceBus;

namespace AdventureWorksCosmos.Core.Commands
{
    public class ProcessDocumentMessages : ICommand
    {
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; }

        // For NSB
        public ProcessDocumentMessages() { }

        private ProcessDocumentMessages(Guid documentId, string documentType)
        {
            DocumentId = documentId;
            DocumentType = documentType;
        }

        public static ProcessDocumentMessages New<TDocument>(TDocument document)
            where TDocument : DocumentBase
        {
            return new ProcessDocumentMessages(document.Id, document.GetType().AssemblyQualifiedName);
        }
    }
}