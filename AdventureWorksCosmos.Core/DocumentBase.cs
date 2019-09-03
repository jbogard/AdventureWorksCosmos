using System;
using System.Collections.Generic;
using AdventureWorksCosmos.Core.Infrastructure;
using Newtonsoft.Json;

namespace AdventureWorksCosmos.Core
{
    public abstract class DocumentBase
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }

        private HashSet<IDocumentMessage> _outbox 
            = new HashSet<IDocumentMessage>(DocumentMessageEqualityComparer.Instance);

        private HashSet<IDocumentMessage> _inbox
            = new HashSet<IDocumentMessage>(DocumentMessageEqualityComparer.Instance);

        public IEnumerable<IDocumentMessage> Outbox
        {
            get => _outbox ?? (_outbox = new HashSet<IDocumentMessage>(DocumentMessageEqualityComparer.Instance));
            protected set => _outbox = value == null
                ? new HashSet<IDocumentMessage>(DocumentMessageEqualityComparer.Instance)
                : new HashSet<IDocumentMessage>(value, DocumentMessageEqualityComparer.Instance);
        }

        public IEnumerable<IDocumentMessage> Inbox
        {
            get => _inbox ?? (_inbox = new HashSet<IDocumentMessage>(DocumentMessageEqualityComparer.Instance));
            protected set => _inbox = value == null
                ? new HashSet<IDocumentMessage>(DocumentMessageEqualityComparer.Instance)
                : new HashSet<IDocumentMessage>(value, DocumentMessageEqualityComparer.Instance);
        }

        protected void Send(
            IDocumentMessage documentMessage) 
            => _outbox.Add(documentMessage);

        protected void Receive<TDocumentMessage>(
            TDocumentMessage documentMessage, 
            Action<TDocumentMessage> action)
            where TDocumentMessage : IDocumentMessage
        {
            if (_inbox.Contains(documentMessage))
                return;

            action(documentMessage);

            _inbox.Add(documentMessage);
        }

        public void Complete(
            IDocumentMessage documentMessage)
        {
            _outbox?.Remove(documentMessage);
        }
    }
}