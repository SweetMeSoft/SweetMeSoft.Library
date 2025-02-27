using Microsoft.Extensions.DependencyInjection;

using SweetMeSoft.Base.Interfaces;
using SweetMeSoft.Middleware.Interface;

namespace SweetMeSoft.Middleware.Service;

public class ServiceFactory(IServiceProvider serviceProvider) : IServiceFactory
{
    public TService Get<TService>() where TService : class, IService
    {
        return serviceProvider.GetService<TService>();
    }
}