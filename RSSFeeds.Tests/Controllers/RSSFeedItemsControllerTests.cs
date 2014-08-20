namespace RSSFeeds.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Moq;
    using NUnit.Framework;
    using RSSFeeds.Controllers;
    using RSSFeeds.Models;
    using RSSFeeds.Services.Repository;
    using RSSFeeds.Services.Repository.EntityFramework;
    using StoryQ;

    [TestFixture]
    public class RSSFeedItemsTests
    {
        public static IRepository<RSSFeed> RSSFeedRepository { get; set; }
        protected IRepository<RSSFeedItem> RSSFeedItemsRepository { get; set; }
        public static IRepository<UserProfile> UserProfileRepository { get; set; }
        private static ControllerContext FakeControllerContext { get; set; }
        private static RssFeedItemsController RSSFeedItemsPage { get; set; }

        public RSSFeedItemsTests()
        {
            RSSFeedRepository = new EfRepository<RSSFeed>();
            UserProfileRepository = new EfRepository<UserProfile>();
            RSSFeedItemsRepository = new EfRepository<RSSFeedItem>();

            var fakeControllerContext = new Mock<ControllerContext>();
            fakeControllerContext.Setup(t => t.HttpContext.User.Identity.Name).Returns("TestUser");
            FakeControllerContext = fakeControllerContext.Object;

            RSSFeedItemsPage = new RssFeedItemsController(UserProfileRepository, RSSFeedRepository, RSSFeedItemsRepository)
                                   {
                                       ControllerContext = FakeControllerContext
                                   };
        }

        #region ShowRSSFeedItems

        private UserProfile showRSSFeedItemsUserProfile;
        private RSSFeed showRSSFeedItemsFeed;
        private ViewResult showRSSFeedItemsFeedListItemsResult;
        private IEnumerable<RSSFeedItem> showRSSFeedItemsFeedListItemsResultItems;
        [Test]
        public void ShowRSSFeedItems()
        {
            new Story("Show RSS Feed Items")
                .InOrderTo("view RSS Feed items details")
                .AsA("User")
                .IWant("A list of RSS Feed Items to be available")

                        .WithScenario("User Lands on RSS Feed Items Page")
                            .Given(TheUserIsOnTheRSSFeedItemsPage)
                                .And(AValidRSSFeedHasBeenSpecifiedForViewing)
                            .When(ThePageLoads)
                            .Then(ItemsForTheSpecifiedRSSFeedShouldBeRetrieved)
                .Execute();
        }
        private void TheUserIsOnTheRSSFeedItemsPage()
        {
            Assert.IsNotNull(RSSFeedItemsPage.User.Identity.Name);
        }
        private void AValidRSSFeedHasBeenSpecifiedForViewing()
        {
            showRSSFeedItemsUserProfile = UserProfileRepository.Get(u => u.UserName == RSSFeedItemsPage.User.Identity.Name).FirstOrDefault();
            Assert.IsNotNull(showRSSFeedItemsUserProfile);

            showRSSFeedItemsFeed = showRSSFeedItemsUserProfile.Feeds.FirstOrDefault();
            Assert.IsNotNull(showRSSFeedItemsFeed);
        }
        private void ThePageLoads()
        {
            showRSSFeedItemsFeedListItemsResult = RSSFeedItemsPage.List(showRSSFeedItemsFeed.RSSFeedUrl);
            Assert.IsNotNull(showRSSFeedItemsFeedListItemsResult);
        }
        private void ItemsForTheSpecifiedRSSFeedShouldBeRetrieved()
        {
            showRSSFeedItemsFeedListItemsResultItems = (showRSSFeedItemsFeedListItemsResult.Model as IEnumerable<RSSFeedItem>);
            Assert.IsNotNull(showRSSFeedItemsFeedListItemsResultItems);
            Assert.IsTrue(showRSSFeedItemsFeedListItemsResultItems.Any());
        }
        
        #endregion

        #region ViewRSSFeedItemDetails

        private UserProfile viewingRSSFeedItemDetailUserProfile;
        private RSSFeed viewingRSSFeedItemDetailFeed;
        private ViewResult viewingRSSFeedItemListResult;
        private IEnumerable<RSSFeedItem> viewingRSSFeedItemListItems;
        private RSSFeedItem viewingRSSFeedItemDetailRssFeedItem;
        private ActionResult viewingRSSFeedItemDetailActionResult;
        private ViewResult viewingRSSFeedItemDetailViewResult;

        [Test]
        public void ViewRSSFeedItemDetails()
        {
            new Story("View RSS Feed Item Details")
                .InOrderTo("have more information available on a given RSS Feed Item")
                .AsA("User")
                .IWant("the detailed feed information to be displayed")

                        .WithScenario("View RSS Feed Item Details")
                            .Given(TheUserIsLoggedIn)
                                .And(TheUserLandsOnTheRSSFeedItemPage)
                            .When(TheUserSelectASpecificRSSFeedItem)
                            .Then(TheUserShouldSeeADetailedViewOfTheRSSFeedItem)
                .Execute();
        }
        private void TheUserIsLoggedIn()
        {
            viewingRSSFeedItemDetailUserProfile = UserProfileRepository.Get(u => u.UserName == RSSFeedItemsPage.User.Identity.Name).FirstOrDefault();
            Assert.IsNotNull(viewingRSSFeedItemDetailUserProfile);
        }
        private void TheUserLandsOnTheRSSFeedItemPage()
        {
            viewingRSSFeedItemDetailFeed = viewingRSSFeedItemDetailUserProfile.Feeds.FirstOrDefault();
            Assert.IsNotNull(viewingRSSFeedItemDetailFeed);

            viewingRSSFeedItemListResult = RSSFeedItemsPage.List(viewingRSSFeedItemDetailFeed.RSSFeedUrl);
            Assert.IsNotNull(viewingRSSFeedItemListResult);
        }
        private void TheUserSelectASpecificRSSFeedItem()
        {
            viewingRSSFeedItemListItems = (viewingRSSFeedItemListResult.Model as IEnumerable<RSSFeedItem>);
            Assert.IsNotNull(viewingRSSFeedItemListItems);

            viewingRSSFeedItemDetailRssFeedItem = viewingRSSFeedItemListItems.FirstOrDefault();
            Assert.IsNotNull(viewingRSSFeedItemDetailRssFeedItem);
        }
        private void TheUserShouldSeeADetailedViewOfTheRSSFeedItem()
        {
            viewingRSSFeedItemDetailActionResult = RSSFeedItemsPage.Detail(viewingRSSFeedItemDetailRssFeedItem.RSSFeedUrl, viewingRSSFeedItemDetailRssFeedItem.RSSFeedItemId);
            Assert.IsNotNull(viewingRSSFeedItemDetailActionResult);

            viewingRSSFeedItemDetailViewResult = viewingRSSFeedItemDetailActionResult as ViewResult;
            Assert.IsNotNull(viewingRSSFeedItemDetailViewResult);

            Assert.IsNotNull(viewingRSSFeedItemDetailViewResult.Model as RSSFeedItem);
        } 

        #endregion

        #region MarkFeedAsRead

        private UserProfile markFeedAsReadUserProfile;
        private RSSFeed markFeedAsReadFeed;
        private ViewResult markFeedAsReadListResult;
        private IEnumerable<RSSFeedItem> markFeedAsReadListItems;
        private RSSFeedItem markFeedAsReadRssFeedItem;
        private RedirectToRouteResult markFeedAsReadRssFeedResult;
        [Test]
        public void MarkFeedAsRead()
        {
            new Story("Mark feed as read")
                .InOrderTo("know which RSS Feed Items have already been read")
                .AsA("User")
                .IWant("to be able to flag a specific RSS Feed Item as read")

                        .WithScenario("Mark RSS Feed Item as read")
                            .Given(TheUserLandsOnTheRSSFeedItemsPage)
                            .When(TheUserClicksTheReadLinkOnASpecificRSSFeedItem)
                            .Then(TheRSSFeedItemShouldBeMarkedAsRead)
                .Execute();
        }
        private void TheUserLandsOnTheRSSFeedItemsPage()
        {
            markFeedAsReadUserProfile = UserProfileRepository.Get(u => u.UserName == RSSFeedItemsPage.User.Identity.Name).FirstOrDefault();
            Assert.IsNotNull(markFeedAsReadUserProfile);

            markFeedAsReadFeed = markFeedAsReadUserProfile.Feeds.FirstOrDefault();
            Assert.IsNotNull(markFeedAsReadFeed);

            markFeedAsReadListResult = RSSFeedItemsPage.List(markFeedAsReadFeed.RSSFeedUrl);
            Assert.IsNotNull(markFeedAsReadListResult);
        }
        private void TheUserClicksTheReadLinkOnASpecificRSSFeedItem()
        {
            markFeedAsReadListItems = (markFeedAsReadListResult.Model as IEnumerable<RSSFeedItem>);
            Assert.IsNotNull(markFeedAsReadListItems);

            markFeedAsReadRssFeedItem = markFeedAsReadListItems.FirstOrDefault();
            Assert.IsNotNull(markFeedAsReadRssFeedItem);
        }
        private void TheRSSFeedItemShouldBeMarkedAsRead()
        {
            markFeedAsReadRssFeedResult = RSSFeedItemsPage.UpdateFeedReadStatus(markFeedAsReadRssFeedItem.RSSFeedItemId, markFeedAsReadRssFeedItem.RSSFeedUrl, true) as RedirectToRouteResult;
            Assert.IsNotNull(markFeedAsReadRssFeedResult);

            var updatedFeed = (from f in markFeedAsReadUserProfile.Feeds
                               where f.RSSFeedUrl == markFeedAsReadFeed.RSSFeedUrl
                               select f.RSSFeedItems.FirstOrDefault( fi => fi.RSSFeedItemId == markFeedAsReadRssFeedItem.RSSFeedItemId && fi.Title == markFeedAsReadRssFeedItem.Title)).FirstOrDefault();
            
            Assert.IsNotNull(updatedFeed);
            Assert.IsTrue(updatedFeed.Read);

            Assert.AreEqual(markFeedAsReadRssFeedResult.RouteValues["rssFeedUrl"], markFeedAsReadRssFeedItem.RSSFeedUrl);
            Assert.AreEqual(markFeedAsReadRssFeedResult.RouteValues["controller"], "RSSFeedItems");
            Assert.AreEqual(markFeedAsReadRssFeedResult.RouteValues["action"], "List");
        }

        #endregion

        #region MarkFeedAsUnread

        private UserProfile markFeedAsUnreadUserProfile;
        private RSSFeed markFeedAsUnreadFeed;
        private ViewResult markFeedAsUnreadListResult;
        private IEnumerable<RSSFeedItem> markFeedAsUnreadListItems;
        private RSSFeedItem markFeedAsUnreadRssFeedItem;
        private RedirectToRouteResult markFeedAsUnreadRssFeedResult;
        [Test]
        public void MarkFeedAsUnread()
        {
            new Story("Mark feed as unread")
                .InOrderTo("know which RSS Feed Items have not been read")
                .AsA("User")
                .IWant("to be able to flag a specific RSS Feed Item as unread")

                        .WithScenario("Mark RSS Feed Item as read")
                            .Given(TheUserLandsOnTheRSSFeedItemsPage1)
                            .When(TheUserClicksTheUnreadLinkOnASpecificRSSFeedItem)
                            .Then(TheRSSFeedItemShouldBeMarkedAsUnread)
                .Execute();
        }
        private void TheUserLandsOnTheRSSFeedItemsPage1()
        {
            markFeedAsUnreadUserProfile = UserProfileRepository.Get(u => u.UserName == RSSFeedItemsPage.User.Identity.Name).FirstOrDefault();
            Assert.IsNotNull(markFeedAsUnreadUserProfile);

            markFeedAsUnreadFeed = markFeedAsUnreadUserProfile.Feeds.FirstOrDefault();
            Assert.IsNotNull(markFeedAsUnreadFeed);

            markFeedAsUnreadListResult = RSSFeedItemsPage.List(markFeedAsUnreadFeed.RSSFeedUrl);
            Assert.IsNotNull(markFeedAsUnreadListResult);
        }
        private void TheUserClicksTheUnreadLinkOnASpecificRSSFeedItem()
        {
            markFeedAsUnreadListItems = (markFeedAsUnreadListResult.Model as IEnumerable<RSSFeedItem>);
            Assert.IsNotNull(markFeedAsUnreadListItems);

            markFeedAsUnreadRssFeedItem = markFeedAsUnreadListItems.FirstOrDefault();
            Assert.IsNotNull(markFeedAsUnreadRssFeedItem);
        }
        private void TheRSSFeedItemShouldBeMarkedAsUnread()
        {
            markFeedAsUnreadRssFeedResult = RSSFeedItemsPage.UpdateFeedReadStatus(markFeedAsUnreadRssFeedItem.RSSFeedItemId, markFeedAsUnreadRssFeedItem.RSSFeedUrl, false) as RedirectToRouteResult;
            Assert.IsNotNull(markFeedAsUnreadRssFeedResult);

            var updatedFeed = (from f in markFeedAsUnreadUserProfile.Feeds
                               where f.RSSFeedUrl == markFeedAsUnreadFeed.RSSFeedUrl
                               select f.RSSFeedItems.FirstOrDefault(fi => fi.RSSFeedItemId == markFeedAsUnreadRssFeedItem.RSSFeedItemId && fi.Title == markFeedAsUnreadRssFeedItem.Title)).FirstOrDefault();

            Assert.IsNotNull(updatedFeed);
            Assert.IsTrue(!updatedFeed.Read);

            Assert.AreEqual(markFeedAsUnreadRssFeedResult.RouteValues["rssFeedUrl"], markFeedAsUnreadRssFeedItem.RSSFeedUrl);
            Assert.AreEqual(markFeedAsUnreadRssFeedResult.RouteValues["controller"], "RSSFeedItems");
            Assert.AreEqual(markFeedAsUnreadRssFeedResult.RouteValues["action"], "List");
        }

        #endregion
    }
}
