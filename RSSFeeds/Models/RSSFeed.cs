namespace RSSFeeds.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RSSFeed
    {
        [MaxLength(50)]
        [Display(Name = "Feed Title")]
        public string RSSFeedTitle { get; set; }

        [Key, Column(Order = 0)]
        [MaxLength(255)]
        public string RSSFeedUrl { get; set; }

        public virtual ICollection<RSSFeedItem> RSSFeedItems { get; set; }

        [Key, Column(Order = 1)]
        public virtual int UserId { get; set; }
        public virtual UserProfile User { get; set; }
    }
}