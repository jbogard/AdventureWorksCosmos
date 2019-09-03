using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace AdventureWorksCosmos.Core.Infrastructure
{
    public class DocumentDBRepository<T> : IDocumentDbRepository<T> where T : DocumentBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly string DatabaseId = typeof(T).Name;
        private readonly string CollectionId = "Items";
        private readonly DocumentClient _client;

        public DocumentDBRepository(IUnitOfWork unitOfWork, DocumentClient client)
        {
            _unitOfWork = unitOfWork;
            _client = client;
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        public async Task<T> GetItemAsync(Guid id)
        {
            try
            {
                var root = _unitOfWork.Find<T>(id);

                if (root != null)
                    return root;

                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id.ToString()));
                var item = (T)(dynamic)document;

                _unitOfWork.Register(item);

                return item;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                    new FeedOptions { MaxItemCount = -1 }).Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }
            _unitOfWork.Register(results);

            return results;
        }

        public async Task<Document> CreateItemAsync(T item)
        {
            _unitOfWork.Register(item);

            var response = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);

            item.ETag = response.Resource.ETag;

            return response;
        }

        public async Task<Document> UpdateItemAsync(T item)
        {
            var ac = new AccessCondition
            {
                Condition = item.ETag,
                Type = AccessConditionType.IfMatch
            };

            var response = await _client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseId, CollectionId, item.Id.ToString()), 
                item,
                new RequestOptions { AccessCondition = ac });

            item.ETag = response.Resource.ETag;

            return response;
        }

        public async Task DeleteItemAsync(Guid id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id.ToString()));
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}