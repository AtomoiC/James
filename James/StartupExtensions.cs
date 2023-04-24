using System.Reflection;
using James.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace James;

public static class StartupExtensions
{
    public static readonly Assembly[] Assemblies = Directory
        .EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "hs.James*.dll")
        .Select(Assembly.LoadFrom)
        .ToArray();

    public static IServiceCollection RegisterServicesByConvention(this IServiceCollection services)
    {
        return services
            .RegisterDependencyServices()
            .RegisterStartupServices();
    }

    public static IServiceCollection RegisterDependencyServices(this IServiceCollection services)
    {
        var serviceTypes = GetClassesWithAttribute<DependencyServiceAttribute>();

        foreach (var service in serviceTypes)
        {
            var attribute = service.GetCustomAttribute<DependencyServiceAttribute>()!;
            var interfaces = service.GetInterfaces();

            Console.WriteLine("Registering {0} as {1} ({2} Interfaces)", service.Name, attribute.Lifetime, interfaces.Length);

            services.RegisterDependencyService(service, service, attribute.Lifetime);

            foreach (var interfaceType in interfaces)
            {
                services.RegisterDependencyService(interfaceType, service, attribute.Lifetime);
            }
        }

        return services;
    }

    public static IServiceCollection RegisterStartupServices(this IServiceCollection services)
    {
        var types = GetClassesWithInterface<IStartupService>();

        foreach (var service in types)
        {
            services.TryAddSingleton(service);
        }

        return services;
    }

    public static IServiceProvider ConfigureStartupServices(this IServiceProvider provider)
    {
        var synchronousStartups = GetClassesWithInterface<IStartupService>()
            .Select(provider.GetRequiredService)
            .Cast<IStartupService>()
            .Select(service => Task.Run(service.OnStartup));

        var asynchronousStartups = GetClassesWithInterface<IAsyncStartupService>()
            .Select(provider.GetRequiredService)
            .Cast<IAsyncStartupService>()
            .Select(service => service.OnStartupAsync());

        Task
            .WhenAll(synchronousStartups.Concat(asynchronousStartups))
            .ConfigureAwait(false);

        return provider;
    }


    private static IEnumerable<Type> GetClassesWithInterface<T>()
    {
        return Assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => typeof(T).IsAssignableFrom(t));
    }

    private static IEnumerable<Type> GetClassesWithAttribute<T>() where T : Attribute
    {
        return Assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetCustomAttribute<T>() is not null);
    }

    private static void RegisterDependencyService(this IServiceCollection services, Type interfaceType, Type serviceType, DependencyLifetime lifetime)
    {
        switch (lifetime)
        {
            case DependencyLifetime.Singleton:
                services.TryAddSingleton(interfaceType, serviceType);
                break;
            case DependencyLifetime.Scoped:
                services.TryAddScoped(interfaceType, serviceType);
                break;
            case DependencyLifetime.Transient:
                services.TryAddTransient(interfaceType, serviceType);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(DependencyServiceAttribute.Lifetime), "Invalid Dependency-Type");
        }
    }

}
