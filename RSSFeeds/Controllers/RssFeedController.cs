namespace RSSFeeds.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using RSSFeeds.Models;
    using RSSFeeds.Services.Repository;

    [Authorize]
    public class RssFeedController : Controller
    {
        public IRepository<RSSFeed> RSSFeedRepo { get; set; }
        public IRepository<UserProfile> UserProfileRepo { get; set; }

        public RssFeedController(IRepository<UserProfile> userProfileRepo, IRepository<RSSFeed> rssFeedRepo)
        {
            this.RSSFeedRepo = rssFeedRepo;
            this.UserProfileRepo = userProfileRepo;
        }

        [HttpGet]
        public ViewResult AddNewRssFeed()
        {
            return this.View();
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewRssFeed(RSSFeed rssFeed)
        {
            if(!ModelState.IsValid)
                return this.View(rssFeed);

            rssFeed.User = UserProfileRepo.Get(u => u.UserName == User.Identity.Name).FirstOrDefault();
            RSSFeedRepo.Add(rssFeed);
            RSSFeedRepo.CommitChanges();
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var user = UserProfileRepo.Get(up => up.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null) 
                return new RedirectResult("~/Account/Login");

            var rssFeeds = user.Feeds.ToList();
            return this.View(rssFeeds);
        }

        public ActionResult DeleteFeed(string rssFeedUrl)
        {
            RSSFeedRepo.Remove(RSSFeedRepo.Get(feed => feed.RSSFeedUrl == rssFeedUrl && feed.User.UserName == User.Identity.Name));
            RSSFeedRepo.CommitChanges();
            return RedirectToAction("List");
        }

        public ActionResult NavigateToRSSFeedItems(RSSFeed firstRssFeed)
        {
            return RedirectToAction("List", "RSSFeedItems", new { rssFeedUrl = firstRssFeed.RSSFeedUrl });
        }

        [HttpGet]
        public ActionResult EditFeed(string rssFeedUrl)
        {
            var rssFeed = RSSFeedRepo.Get(f => f.RSSFeedUrl == rssFeedUrl && f.User.UserName == User.Identity.Name).FirstOrDefault();
            return View(rssFeed);
        }

        [HttpPost]
        public ActionResult EditFeed(RSSFeed rssFeed)
        {
            RSSFeedRepo.Update(rssFeed, rssFeed.RSSFeedUrl, rssFeed.UserId);
            RSSFeedRepo.CommitChanges();
            return RedirectToAction("List");
        }
    }
}