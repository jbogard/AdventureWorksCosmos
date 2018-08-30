using System;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Core.Models.Orders;
using MediatR;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.Documents.ChangeFeedProcessor.DataAccess;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using NServiceBus;
using StructureMap;

namespace AdventureWorksCosmos.Dispatcher
{
    class Program
    {
        private const string HostName = "AdventureWorksCosmos.Dispatcher";
        private static readonly string CosmosUrl = "https://localhost:8081/";
        private static readonly string CosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public static Container Container { get; private set; }
        public static IEndpointInstance Endpoint { get; private set; }

        static async Task Main()
        {
            var client = new DocumentClient(new Uri(CosmosUrl), CosmosKey, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            Container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<IDocumentMessage>();
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IDocumentMessageHandler<>));
                });


                cfg.For<IMediator>().Use<Mediator>();
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);

                cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(UnitOfWorkBehavior<,>));
                cfg.For(typeof(IDocumentDBRepository<>)).Use(typeof(DocumentDBRepository<>));
                cfg.For<IUnitOfWork>().Use<UnitOfWork>();
                cfg.For<DocumentClient>().Use(client);
                cfg.For<IDocumentMessageDispatcher>().Use<DocumentMessageDispatcher>();
                cfg.For<IOfflineDispatcher>().Use<UniformSessionOfflineDispatcher>();
            });

            var endpointConfiguration = new EndpointConfiguration(HostName);

            endpointConfiguration.UseContainer<StructureMapBuilder>(customizations => customizations.ExistingContainer(Container));
            endpointConfiguration.EnableUniformSession();

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString("host=localhost");

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            Endpoint = await NServiceBus.Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            var builder = CreateBuilder<OrderRequest>(client);
            var processor = await builder.BuildAsync();

            await processor.StartAsync();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await Endpoint.Stop()
                .ConfigureAwait(false);

            await processor.StopAsync();
        }

        private static ChangeFeedProcessorBuilder CreateBuilder<T>(DocumentClient client) 
            where T : DocumentBase
        {
            var builder = new ChangeFeedProcessorBuilder();
            var uri = new Uri(CosmosUrl);
            var dbClient = new ChangeFeedDocumentClient(client);

            builder
                .WithHostName(HostName)
                .WithFeedCollection(new DocumentCollectionInfo
                {
                    DatabaseName = typeof(T).Name,
                    CollectionName = "Items",
                    Uri = uri,
                    MasterKey = CosmosKey
                })
                .WithLeaseCollection(new DocumentCollectionInfo
                {
                    DatabaseName = typeof(T).Name,
                    CollectionName = "Leases",
                    Uri = uri,
                    MasterKey = CosmosKey
                })
                .WithProcessorOptions(new ChangeFeedProcessorOptions
                {
                    FeedPollDelay = TimeSpan.FromSeconds(15),
                })
                .WithFeedDocumentClient(dbClient)
                .WithLeaseDocumentClient(dbClient)
                .WithObserver<DocumentFeedObserver<T>>();

            return builder;
        }
    }
}