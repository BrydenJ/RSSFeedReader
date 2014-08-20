namespace RSSFeeds.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Xml;
    using RSSFeeds.Models;

    public static class RSSFeedItemHelper
    {
        public static IEnumerable<RSSFeedItem> GetRSSFeedItems(RSSFeed rssFeed, UserProfile userProfile)
        {
            var feed = SyndicationFeed.Load(XmlReader.Create(rssFeed.RSSFeedUrl));
            
            if (feed == null)
                throw new Exception("The Rss Feed URL is not valid");

            return feed.Items.Select
                (
                    item => new RSSFeedItem
                                {
                                    RSSFeedItemId = item.Id, 
                                    Title = item.Title.Text, 
                                    Summary = item.Summary.Text, 
                                    Read = false, 
                                    RSSFeed = rssFeed, 
                                    User = userProfile
                                }).ToList();
        }
    }
}