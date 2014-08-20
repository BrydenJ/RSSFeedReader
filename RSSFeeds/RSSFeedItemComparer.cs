namespace RSSFeeds
{
    using System.Collections.Generic;

    using RSSFeeds.Models;

    public class RSSFeedItemComparer : IEqualityComparer<RSSFeedItem>
    {
        public bool Equals(RSSFeedItem x, RSSFeedItem y)
        {
            return x.Title == y.Title &&
                   x.RSSFeedItemId == y.RSSFeedItemId;
        }

        public int GetHashCode(RSSFeedItem obj)
        {
            return -1;
        }
    }
}