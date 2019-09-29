using Domain.Account;
using Domain.BestExecution;
using Domain.Currency;
using Domain.FxExposure;
using Domain.FxOrders;
using Domain.Quote;
using Gateway.Middleware.LegacyWiring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace Gateway.TradingApi.Web.Middleware
{
    public static class ServiceCollectionExtensions
    {
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
