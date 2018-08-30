using System;
using AdventureWorksCosmos.Core;
using AdventureWorksCosmos.Core.Infrastructure;
using AdventureWorksCosmos.Products.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.UniformSession;
using StructureMap;

namespace AdventureWorksCosmos.UI
{
    public class Startup
    {
        private const string CosmosUrl = "https://localhost:8081/";
        private const string CosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IEndpointInstance Endpoint { get; private set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDistributedMemoryCache();

            var client = new DocumentClient(new Uri(CosmosUrl), CosmosKey, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<IDocumentMessage>();
                    scanner.ConnectImplementationsToTypesClosing(typeof(IDocumentMessageHandler<>));
                });

                cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(UnitOfWorkBehavior<,>));
                cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(RetryUnitOfWorkBehavior<,>));
                cfg.For(typeof(IDocumentDBRepository<>)).Use(typeof(DocumentDBRepository<>));
                cfg.For<IUnitOfWork>().Use<UnitOfWork>();
                cfg.For<IDocumentMessageDispatcher>().Use<DocumentMessageDispatcher>();
                cfg.For<IOfflineDispatcher>().Use<UniformSessionOfflineDispatcher>();
                cfg.For<IMessageSession>().Singleton().Use(() => Endpoint);
                cfg.For<DocumentClient>().Use(client);
            });

            var endpointConfiguration = new EndpointConfiguration("AdventureWorksCosmos.UI");
            endpointConfiguration.UseContainer<StructureMapBuilder>(c => c.ExistingContainer(container));

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString("host=localhost");
            endpointConfiguration.SendOnly();
            endpointConfiguration.EnableUniformSession();

            var routing = transport.Routing();
            routing.RouteToEndpoint(
                assembly: typeof(IDocumentMessage).Assembly,
                destination: "AdventureWorksCosmos.Dispatcher");


            services.AddMediatR(typeof(Startup), typeof(IDocumentMessage));

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(300);
                options.Cookie.HttpOnly = true;
            });

            services.AddDbContext<AdventureWorks2016Context>();

            container.Populate(services);

            Endpoint = NServiceBus.Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSession();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        void OnShutdown()
        {
            Endpoint?.Stop().GetAwaiter().GetResult();
        }
    }

}
