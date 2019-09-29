using Domain.Account;
using Domain.BestExecution;
using Domain.Currency;
using Domain.FxExposure;
using Domain.FxOrders;
using Domain.Quote;
using Gateway.Middleware.LegacyWiring;
using Gateway.TradingApi.Queue.Consumers;
using Gateway.TradingApi.Queue.Model.Settings;
using Gateway.TradingApi.Queue.Services;
using Gateway.TradingApi.Queue.Services.Background;
using GreenPipes;
using MassTransit;
using MassTransit.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;

namespace Gateway.TradingApi.Queue.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// This 'dirty' trick allows us to register a host service as an injectable singleton API which strictly speaking is a bit of a no-no.
        /// Have a read of this thread for more details: https://github.com/aspnet/Extensions/issues/553
        /// </summary>
        public static IServiceCollection AddHostedService<TService, T>(this IServiceCollection services)
            where TService : class
            where T : class, TService, IHostedService
        {
            services.AddSingleton<T>();
            services.AddSingleton<IHostedService>(x => x.GetRequiredService<T>());
            services.AddSingleton<TService>(x => x.GetRequiredService<T>());

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            //Settings config
            services.Configure<GatewayHostTradingApiQueueSettings>(
                configuration.GetSection(nameof(GatewayHostTradingApiQueueSettings)));
            services.Configure<RabbitMqSettings>(configuration.GetSection(nameof(RabbitMqSettings)));

            //hosted services config
            services.AddHostedService<RabbitMqService>();

            //other registrations
            services.AddTransient<IFxRequestExecutionProcessor, FxRequestExecutionProcessor>();

            return services;
        }

        public static IServiceCollection AddMassTransitRabbitMq(this IServiceCollection source,
            IConfiguration configuration)
        {
            source.AddMassTransit(x =>
            {
                MessageCorrelation.UseCorrelationId<IResponse<FxBestExecutionResponse>>(c => c.GetCorrelationId());
                x.AddConsumer<FxOrderRequestConsumer>();

                var serviceProvider = source.BuildServiceProvider();
                var rabbitMqSettings = serviceProvider.GetService<IOptions<RabbitMqSettings>>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseExtensionsLogging(provider.GetRequiredService<ILoggerFactory>());

                    var host = cfg.Host(
                        new Uri($"rabbitmq://{rabbitMqSettings.Value.Host}/{rabbitMqSettings.Value.VirtualHost}"),
                        hostConfigurator =>
                        {
                            hostConfigurator.Username(rabbitMqSettings.Value.UserName);
                            hostConfigurator.Password(rabbitMqSettings.Value.Password);
                        });

                    cfg.ReceiveEndpoint(host, rabbitMqSettings.Value.FxOrderRequestEndPoint, ep =>
                    {
                        ep.PrefetchCount = 2;
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.Consumer<FxOrderRequestConsumer>(provider);
                    });
                }));
            });

            return source;
        }

        /// <summary>
        /// This is the single point that we bring in all the legacy portfolio dependencies
        /// </summary>
        public static IServiceCollection AddLegacyPortfolioCollateral(this IServiceCollection services,
                                                                        IConfigurationRoot configuration)
        {
            services.AddTransient<IPrincipal, UniversalPrincipal>();
            bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary)
            {
                return compilationLibrary.Name == "BTCS" ||
                       compilationLibrary.Dependencies.Any(d => d.Name.StartsWith("BTCS")) ||
                       compilationLibrary.Dependencies.Any(d => d.Name.StartsWith("Domain.Services"));
            }

            var bootstrap = new BTCS.Bank.Business.BootstrapServices();
            bootstrap.ConfigureServices(configuration, services);

            return services.AddAutoMapper(DependencyContext.Default
                                                        .RuntimeLibraries
                                                        .Where(IsCandidateCompilationLibrary)
                                                        .Select(l => Assembly.Load(new AssemblyName(l.Name)))
                                                        .ToArray());
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<FxOrderService>();
            services.AddTransient<IFxBestExecutionService, FxBestExecutionService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IFxQuoteService, FxQuoteService>();
            services.AddTransient<ICurrencyService, CurrencyService>();
            services.AddTransient<IFxExposureService, FxExposureService>();

            return services;
        }
    }
}
