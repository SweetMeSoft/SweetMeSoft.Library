using SweetMeSoft.Base.Interfaces;

using System.Linq.Expressions;

namespace SweetMeSoft.Middleware.Interface;

public interface IRepository
{
    Task<int> Count<T>(Expression<Func<T, bool>> where = null) where T : class, IDBEntity;

    Task<bool> Delete<T>(T entity) where T : class, IDBEntity;

    Task<bool> Delete<T, TKey>(TKey key) where T : class, IDBEntity, new();

    Task<int> Delete<T>(Expression<Func<T, bool>> where) where T : class, IDBEntity;

    Task<IEnumerable<T>> ExecuteSelectQuery<T>(string query) where T : class, IDBEntity, new();

    Task<IEnumerable<T>> GetAll<T>(params string[] routes) where T : class, IDBEntity, new();

    Task<IEnumerable<T>> GetByField<T>(Expression<Func<T, bool>> where, params string[] routes) where T : class, IDBEntity, new();

    Task<IEnumerable<T>> GetByFieldAsc<T, TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy, int take, params string[] routes) where T : class, IDBEntity, new();

    Task<IEnumerable<T>> GetByFieldDesc<T, TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy, int take, params string[] routes) where T : class, IDBEntity, new();

    Task<T> GetById<T, TKey>(TKey key, params string[] routes) where T : class, IDBEntity, new();

    Task<IEnumerable<string>> GetTableNames();

    Task<bool> InsertItem<T>(T obj) where T : class, IDBEntity;

    Task<int> InsertList<T>(IEnumerable<T> list) where T : class, IDBEntity;

    Task<T> Single<T>(Expression<Func<T, bool>> predicate, params string[] routes) where T : class, IDBEntity, new();

    Task<bool> Update<T>(T entity) where T : class, IDBEntity;

    Task<int> Update<T>(T obj, Expression<Func<T, bool>> where) where T : class, IDBEntity;

    Task<int> Update<T, TKey>(T original, T edited, Expression<Func<T, TKey>> key) where T : class, IDBEntity;

    Task<bool> UpdateList<T>(IEnumerable<T> entity) where T : class, IDBEntity;
}