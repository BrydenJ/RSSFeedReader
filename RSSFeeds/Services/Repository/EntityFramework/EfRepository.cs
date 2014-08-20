namespace RSSFeeds.Services.Repository.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    using RSSFeeds.Services.Repository;

    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> dbSet;
        private readonly EfUnitOfWork uow = EfUnitOfWork.Current;

        public EfRepository()
        {
            if (uow == null) throw new Exception("Must be EF Unit Of Work");
            this.dbSet = (uow).GetDbSet<T>();
        }

        public void Add(T item)
        {
            this.dbSet.Add(item);
        }

        public void Add(IEnumerable<T> items)
        {
            this.dbSet.AddRange(items);
        }

        public void Remove(T item)
        {
            this.dbSet.Remove(item);
        }

        public void Remove(IEnumerable<T> items)
        {
            this.dbSet.RemoveRange(items);
        }

        public void Update(T item, params object[] keyValues)
        {
            var foundItem = dbSet.Find(keyValues);
            EfContext.Current.Entry(foundItem).CurrentValues.SetValues(item);
        }

        public void Update(Dictionary<object[], T> items)
        {
            foreach (var item in items)
            {
                Update(item.Value, item.Key);
            }
        }

        public void CommitChanges()
        {
            EfUnitOfWork.Current.Commit();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = this.dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }
    }

}