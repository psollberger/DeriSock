namespace DeriSock.DevTools;

using System.Linq;

using Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
  public static IServiceCollection AddUseCases(this IServiceCollection services)
  {
    var useCaseTypes = typeof(HostUtils).Assembly.GetTypes().Where(x => x.GetInterface(nameof(IUseCase)) is not null);

    foreach (var useCaseType in useCaseTypes)
      services.AddTransient(useCaseType, useCaseType);

    return services;
  }
}
