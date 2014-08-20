namespace RSSFeeds.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RSSFeedItem
    {
        [Key, Column(Order = 0)]
        public string RSSFeedItemId { get; set; }
        [Key, Column(Order = 1)]
        public string Title { get; set; }
        public string Summary { get; set; }

        public virtual int UserId { get; set; }
        public virtual UserProfile User { get; set; }

        public virtual string RSSFeedUrl { get; set; }
        public virtual RSSFeed RSSFeed { get; set; }

        public bool Read { get; set; }
    }
}