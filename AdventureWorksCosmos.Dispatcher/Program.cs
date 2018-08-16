using System;
using System.Threading.Tasks;
using AdventureWorksCosmos.Core;
using AdventureWorksCosmos.Core.Infrastructure;
using MediatR;
using NServiceBus;
using StructureMap;
using StructureMap.Pipeline;

namespace AdventureWorksCosmos.Dispatcher
{
    class Program
    {
        static async Task Main()
        {
            var container = new Container(cfg =>
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
                cfg.For<IDocumentMessageDispatcher>().Use<DocumentMessageDispatcher>();
                cfg.For<IOfflineDispatcher>().Use<UniformSessionOfflineDispatcher>();
            });

            var endpointConfiguration = new EndpointConfiguration("AdventureWorksCosmos.Dispatcher");

            endpointConfiguration.UseContainer<StructureMapBuilder>(customizations => customizations.ExistingContainer(container));
            endpointConfiguration.EnableUniformSession();

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString("host=localhost");

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}