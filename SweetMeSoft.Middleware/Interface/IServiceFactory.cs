using SweetMeSoft.Base.Interfaces;

namespace SweetMeSoft.Middleware.Interface;

public interface IServiceFactory
{
    TService Get<TService>() where TService : class, IService;
}