namespace RSSFeeds.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using RSSFeeds.Helpers;
    using RSSFeeds.Models;
    using RSSFeeds.Services.Repository;

    public class RssFeedItemsController : Controller
    {
        private UserProfile UserProfile {
            get
            {
                return UserProfileRepository.Get(user => user.UserName == User.Identity.Name).FirstOrDefault();
            }
        }
        public IRepository<RSSFeed> RSSFeedRepository { get; set; }
        public IRepository<RSSFeedItem> RSSFeedItemRepository { get; set; }
        public IRepository<UserProfile> UserProfileRepository { get; set; }

        public RssFeedItemsController(IRepository<UserProfile> userProfileRepository, IRepository<RSSFeed> rssFeedRepository, IRepository<RSSFeedItem> rssFeedItemRepository)
        {
            RSSFeedRepository = rssFeedRepository;
            RSSFeedItemRepository = rssFeedItemRepository;
            UserProfileRepository = userProfileRepository;
        }

        public ViewResult List(string rssFeedUrl)
        {
            var rssFeed = this.UserProfile.Feeds.FirstOrDefault(f => f.RSSFeedUrl == rssFeedUrl);
            if (rssFeed == null)
                throw new Exception("The specified RSS feed could not be found");

            var rssFeedItems = RSSFeedItemHelper.GetRSSFeedItems(rssFeed, UserProfile).ToList();
            var itemsNotInRepo = rssFeedItems.Except(rssFeed.RSSFeedItems, new RSSFeedItemComparer());
            if (itemsNotInRepo.Any())
            {
                RSSFeedItemRepository.Add(itemsNotInRepo);
                RSSFeedItemRepository.CommitChanges();
            }

            var feedItems = rssFeed.RSSFeedItems;
            return View(feedItems);
        }

        [HttpGet]
        public ActionResult Detail(string rssFeedUrl, string rssFeedItemId)
        {
            var feed = (from f in UserProfile.Feeds
                       where f.RSSFeedUrl == rssFeedUrl
                        select f.RSSFeedItems.FirstOrDefault(fi => fi.RSSFeedItemId == rssFeedItemId)).FirstOrDefault();
            if (feed == null)
                throw new Exception("The specified RSS Feed Item could not be found");
            if (!feed.Read)
            {
                feed.Read = true;
                RSSFeedItemRepository.Update(feed, feed.RSSFeedItemId, feed.Title);
            }
            return this.View(feed);
        }

        public ActionResult UpdateFeedReadStatus(string rssFeedItemId, string rssFeedUrl, bool read)
        {
            var feed = (from f in UserProfile.Feeds
                       where f.RSSFeedUrl == rssFeedUrl
                       select f.RSSFeedItems.FirstOrDefault(fi => fi.RSSFeedItemId == rssFeedItemId)).FirstOrDefault();
            if (feed == null)
                throw new Exception("The specified RSS Feed Item could not be found");

            feed.Read = read;
            this.RSSFeedItemRepository.Update(feed, feed.RSSFeedItemId, feed.Title);

            return RedirectToAction("List", "RSSFeedItems", new { rssFeedUrl = feed.RSSFeedUrl });
        }
    }
}