using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Middleware.LegacyWiring
{
    /// <summary>
    /// This registration code presumably supports portfolio code
    /// </summary>
    public static class AutoMapperServiceCollectionRegistration
    {
        private static readonly Action<IMapperConfigurationExpression> DefaultConfig = cfg => { };

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
            => AddAutoMapperClasses(services, null, assemblies);

        private static IServiceCollection AddAutoMapperClasses(IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Assembly> assembliesToScan)
        {
            additionalInitAction = additionalInitAction ?? DefaultConfig;
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            var allTypes = assembliesToScan
                .Where(a => a.GetName().Name != nameof(AutoMapper))
                .SelectMany(a => a.DefinedTypes)
                .ToArray();

            var profiles =
                allTypes
                    .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t))
                    .Where(t => !t.IsAbstract);

            Mapper.Initialize(cfg =>
            {
                additionalInitAction(cfg);

                foreach (var profile in profiles.Select(t => t.AsType()))
                {
                    cfg.AddProfile(profile);
                }
            });

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>)
            };
            foreach (var type in openTypes.SelectMany(openType => allTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => t.AsType().ImplementsGenericInterface(openType))))
            {
                services.AddTransient(type.AsType());
            }

            services.AddSingleton(Mapper.Configuration);
            return services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            return type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));
        }

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
