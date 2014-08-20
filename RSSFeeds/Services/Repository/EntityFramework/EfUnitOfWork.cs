namespace RSSFeeds.Services.Repository.EntityFramework
{
    using System.Data.Entity;

    using RSSFeeds.Services.Repository;

    public class EfUnitOfWork : IUnitOfWork
    {
        private static EfUnitOfWork currentUnitOfWork;
        public static EfUnitOfWork Current { get { return currentUnitOfWork ?? (currentUnitOfWork = new EfUnitOfWork()); } }

        internal DbSet<T> GetDbSet<T>() where T : class
        {
            return EfContext.Current.Set<T>();
        }

        public void Commit()
        {
            EfContext.Current.SaveChanges();
        }
        public void Dispose()
        {
            EfContext.Current.Dispose();
        }
    }
}