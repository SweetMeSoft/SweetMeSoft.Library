using Microsoft.Extensions.DependencyInjection;

using HughesNet.Base.Interfaces;
using HughesNet.Middleware.Interface;

namespace HughesNet.Middleware.Service;

public class ServiceFactory(IServiceProvider serviceProvider) : IServiceFactory
{
    public TService Get<TService>() where TService : class, IService
    {
        return serviceProvider.GetService<TService>();
    }
}