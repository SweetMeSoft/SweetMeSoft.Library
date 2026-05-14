using HughesNet.Base.Interfaces;

namespace HughesNet.Middleware.Interface;

public interface IServiceFactory
{
    TService Get<TService>() where TService : class, IService;
}