namespace RSSFeeds.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using RSSFeeds.Filters;
    using RSSFeeds.Models;
    using RSSFeeds.Services.Repository.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<EfContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        [InitializeSimpleMembership]
        protected override void Seed(EfContext context)
        {
            var feedUser = context.UserProfiles.FirstOrDefault(user => user.UserName == "TestUser");
            if (feedUser == null)
                throw new Exception("Comment the seed section out, start the application and first create a user called TestUser");

            context.RSSFeeds.AddOrUpdate(
                feed => feed.RSSFeedTitle,
                new RSSFeed
                    {
                        RSSFeedTitle = "Miami Herald - Pets",
                        RSSFeedUrl = "http://www.miamiherald.com/living/pets/index.xml",
                        User = feedUser
                    },
                new RSSFeed
                    {
                        RSSFeedTitle = "Miami Herald - Food",
                        RSSFeedUrl = "http://www.miamiherald.com/living/food/index.xml",
                        User = feedUser
                    },
                new RSSFeed
                    {
                        RSSFeedTitle = "Miami Herald - Health",
                        RSSFeedUrl = "http://www.miamiherald.com/living/health/index.xml",
                        User = feedUser
                    },
                new RSSFeed
                    {
                        RSSFeedTitle = "Miami Herald - Home",
                        RSSFeedUrl = "http://www.miamiherald.com/living/home/index.xml",
                        User = feedUser
                    },
                new RSSFeed
                    {
                        RSSFeedTitle = "Miami Herald - Fashion",
                        RSSFeedUrl = "http://www.miamiherald.com/living/fashion/index.xml",
                        User = feedUser
                    },
                new RSSFeed
                    {
                        RSSFeedTitle = "Miami Herald - Travel",
                        RSSFeedUrl = "http://www.miamiherald.com/living/travel/index.xml",
                        User = feedUser
                    }
                );
        }
    }
}
