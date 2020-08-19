using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Currency.Common.DI
{
    public static class AddAssemblyRegister
    {
        public static void AddAssembly(this IServiceCollection service, string assemblyName
            , ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            var assembly = RuntimeHelper.GetAssemblyByName(assemblyName);

            var types = assembly.GetTypes();
            var list = types.Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType).ToList();

            foreach (var type in list)
            {
                var interfaceList = type.GetInterfaces();
                if (interfaceList.Any())
                {
                    var inter = interfaceList.First();

                    switch (serviceLifetime)
                    {
                        case ServiceLifetime.Transient:
                            service.AddTransient(inter, type);
                            break;
                        case ServiceLifetime.Scoped:
                            service.AddScoped(inter, type);
                            break;
                        case ServiceLifetime.Singleton:
                            service.AddSingleton(inter, type);
                            break;

                    }
                    //service.AddScoped(inter, type);
                }
            }
        }
    }
    public class RuntimeHelper
    {
        //通过程序集的名称加载程序集
        public static Assembly GetAssemblyByName(string assemblyName)
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
        }
    }

}
