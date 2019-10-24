using System.Linq;
using Calculator.CountingService.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace Calculator.CountingService.Extensions
{
    public static class CountingServiceExtensions
    {
        public static IServiceCollection AddOperations(this IServiceCollection serviceCollection)
        {
            var operationInterfaceType = typeof(IOperation);
            var operationsAssembly = operationInterfaceType.Assembly;

            var typesToRegister = operationsAssembly
                .GetTypes()
                .Where(type => operationInterfaceType.IsAssignableFrom(type) && !type.IsInterface);

            foreach (var type in typesToRegister)
            {
                serviceCollection.AddSingleton(operationInterfaceType, type);
            }

            return serviceCollection;
        }
    }
}
