namespace RSSFeeds.Services.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }

    public interface IRepository<T>
    {
        void Add(T item);
        void Add(IEnumerable<T> item);
        void Remove(T item);
        void Remove(IEnumerable<T> item);
        void Update(T item, params object[] keyValues);
        void Update(Dictionary<object[], T> items);
        void CommitChanges();
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,string includeProperties = "");
    }

}