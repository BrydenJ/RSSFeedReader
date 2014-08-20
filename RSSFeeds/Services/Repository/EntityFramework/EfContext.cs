namespace RSSFeeds.Services.Repository.EntityFramework
{
    using System.Data.Entity;

    using RSSFeeds.Models;

    public class EfContext : DbContext
    {
        public EfContext() : base("DefaultConnection")
        {
            var _ = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }
        private static EfContext currentContext;
        public static EfContext Current { get{return currentContext ?? (currentContext = new EfContext());} }
        
        public DbSet<RSSFeed> RSSFeeds { get; set; }
        public DbSet<RSSFeedItem> RSSFeedItems { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<EfContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}
