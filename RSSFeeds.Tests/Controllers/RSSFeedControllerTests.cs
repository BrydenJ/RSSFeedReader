namespace RSSFeeds.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Mvc;
    using Moq;
    using NUnit.Framework;
    using RSSFeeds.Controllers;
    using RSSFeeds.Models;
    using System.Linq;
    using RSSFeeds.Services.Repository;
    using RSSFeeds.Services.Repository.EntityFramework;
    using StoryQ;

    [TestFixture]
    public class RSSFeedTests
    {
        public static IRepository<RSSFeed> RSSFeedRepository { get; set; }
        public static IRepository<UserProfile> UserProfileRepository { get; set; }
        private static ControllerContext FakeControllerContext { get; set; }
        private static RssFeedController RSSFeedPage { get; set; }

        public RSSFeedTests()
        {
            RSSFeedRepository = new EfRepository<RSSFeed>();
            UserProfileRepository = new EfRepository<UserProfile>();

            var fakeControllerContext = new Mock<ControllerContext>();
            fakeControllerContext.Setup(t => t.HttpContext.User.Identity.Name).Returns("TestUser");
            FakeControllerContext = fakeControllerContext.Object;

            RSSFeedPage = new RssFeedController(UserProfileRepository, RSSFeedRepository) { ControllerContext = FakeControllerContext };
        }
        
        #region Shared

        private void TheUserIsOnTheRSSFeedListPage()
        {
            Assert.IsNotNull(RSSFeedPage, "The Rss feed page must be defined");
            Assert.IsNotNull(RSSFeedPage.User.Identity.Name);
        }

        #endregion

        #region AddANewRSSFeed

        private RSSFeed addANewRSSFeedRssFeed;
        [Test]
        public void AddANewRSSFeed()
        {
            new Story("Add a new RSS Feed").InOrderTo("retrieve content and view a list of items in the feed")
                                           .AsA("User")
                                           .IWant("to be able to add a new feed to the FeedList")

                                           .WithScenario("Add valid rss feed")
                                               .Given(TheRelevantRssFeedHasBeenCapturedForASpecificUser)
                                               .When(TheRssFeedIsSubmitted)
                                               .Then(TheRssFeedShouldBeSavedForLaterUse)

                                           .Execute();
        }
        private void TheRelevantRssFeedHasBeenCapturedForASpecificUser()
        {
            addANewRSSFeedRssFeed = new RSSFeed { RSSFeedTitle = "Title", RSSFeedUrl = "http://Url" };
        }
        private void TheRssFeedIsSubmitted()
        {
            RSSFeedPage.AddNewRssFeed(addANewRSSFeedRssFeed);
        }
        private void TheRssFeedShouldBeSavedForLaterUse()
        {
            var feed = RSSFeedRepository.Get(f => f.RSSFeedUrl == addANewRSSFeedRssFeed.RSSFeedUrl).FirstOrDefault();
            Assert.IsNotNull(feed);
        }

        #endregion

        #region ViewingFeedList

        private ViewResult viewingFeedListListResult;
        private IEnumerable<RSSFeed> viewingFeedListRssFeeds;
        [Test]
        public void ViewingFeedList()
        {
            new Story("Viewing Feed List")
                .InOrderTo("have the ability to maintain feeds and view feed items")
                .AsA("User")
                .IWant("to be able to view a list of feeds previously added")

                        .WithScenario("Viewing a list of feeds")
                            .Given(TheUserIsLoggedIn)
                            .When(TheUserLandsOnTheRssFeedPage)
                            .Then(TheUsersRssFeedsShouldBeDisplayedOnTheRssFeedPage)
                .Execute();
        }
        private void TheUserIsLoggedIn()
        {
            Assert.IsNotNull(RSSFeedPage.User.Identity.Name);
        }
        private void TheUserLandsOnTheRssFeedPage()
        {
            Assert.IsNotNull(RSSFeedPage, "The Rss feed page must be defined");
            viewingFeedListListResult = RSSFeedPage.List() as ViewResult;
            Assert.IsNotNull(viewingFeedListListResult);
        }
        private void TheUsersRssFeedsShouldBeDisplayedOnTheRssFeedPage()
        {
            viewingFeedListRssFeeds = (viewingFeedListListResult.Model as IEnumerable<RSSFeed>);
            Assert.IsNotNull(viewingFeedListRssFeeds);
            Assert.IsTrue(viewingFeedListRssFeeds.Any());
        }

        #endregion

        #region DeleteRSSFeed

        private ViewResult deleteRSSFeedListResult;
        private IEnumerable<RSSFeed> deleteRSSFeedRssFeeds;
        private RSSFeed deleteRSSFeedRssFeedToBeDeleted;
        [Test]
        public void DeleteRSSFeed()
        {
            new Story("Delete RSS Feed")
                .InOrderTo("Ensure that content is not received for unwanted RSS feeds")
                .AsA("User")
                .IWant("to be able to delete a RSS feed from the FeedList")

                .WithScenario("Delete an existing rss feed")
                .Given(TheUserIsOnTheRSSFeedListPage)
                .When(TheUserClicksDeleteOnASpecificFeed)
                .Then(TheFeedShouldBeDeletedFromTheFeedList)
                .Execute();

        }
        private void TheUserClicksDeleteOnASpecificFeed()
        {
            deleteRSSFeedListResult = RSSFeedPage.List() as ViewResult;
            Assert.IsNotNull(deleteRSSFeedListResult, "The list of feeds could not be retrieved");

            deleteRSSFeedRssFeeds = (deleteRSSFeedListResult.Model as IEnumerable<RSSFeed>);
            Assert.IsNotNull(deleteRSSFeedRssFeeds);

            deleteRSSFeedRssFeedToBeDeleted = deleteRSSFeedRssFeeds.FirstOrDefault();
            Assert.IsNotNull(deleteRSSFeedRssFeedToBeDeleted);

            RSSFeedPage.DeleteFeed(deleteRSSFeedRssFeedToBeDeleted.RSSFeedUrl);
        }
        private void TheFeedShouldBeDeletedFromTheFeedList()
        {
            var feed = RSSFeedRepository.Get(feedInRepo => feedInRepo.RSSFeedUrl == deleteRSSFeedRssFeedToBeDeleted.RSSFeedUrl).FirstOrDefault();
            Assert.IsNull(feed, "The feed was not removed from the repository");
        }

        #endregion

        #region EditRSSFeed

        private ViewResult editRSSFeedListResult;
        private IEnumerable<RSSFeed> editRSSFeedRssFeeds;
        private RSSFeed rssFeedRssFeedToBeEdited;
        private ActionResult editRSSFeedItemResult;
        [Test]
        public void EditRSSFeed()
        {
            new Story("Edit RSS Feed")
                .InOrderTo("Ensure that content on a RSS feed is correct")
                .AsA("User")
                .IWant("to be able to edit an existing RSS feed in the FeedList")

                        .WithScenario("Edit an existing RSS feed")
                            .Given(TheUserIsOnTheRSSFeedListPage)
                                .And(TheUserHasClickedEditOnASpecificFeed)
                            .When(TheUserEditsTheFeedInformationAndSubmitsThePage)
                            .Then(TheFeedDetailsShouldBeUpdatedInTheFeedList)
                .Execute();
        }
        private void TheUserHasClickedEditOnASpecificFeed()
        {
            editRSSFeedListResult = RSSFeedPage.List() as ViewResult;
            Assert.IsNotNull(editRSSFeedListResult, "The list of feeds could not be retrieved");

            editRSSFeedRssFeeds = (editRSSFeedListResult.Model as IEnumerable<RSSFeed>);
            Assert.IsNotNull(editRSSFeedRssFeeds);

            rssFeedRssFeedToBeEdited = editRSSFeedRssFeeds.FirstOrDefault();
            Assert.IsNotNull(rssFeedRssFeedToBeEdited);

            editRSSFeedItemResult = RSSFeedPage.EditFeed(rssFeedRssFeedToBeEdited.RSSFeedUrl);
            Assert.IsNotNull(editRSSFeedItemResult);
            Assert.IsTrue(editRSSFeedItemResult is ViewResult);
        }
        private void TheUserEditsTheFeedInformationAndSubmitsThePage()
        {
            rssFeedRssFeedToBeEdited.RSSFeedTitle = "NewTitle";
            editRSSFeedItemResult = RSSFeedPage.EditFeed(rssFeedRssFeedToBeEdited);
        }
        private void TheFeedDetailsShouldBeUpdatedInTheFeedList()
        {
            var feedInRepo = RSSFeedRepository.Get(f => f.RSSFeedUrl == rssFeedRssFeedToBeEdited.RSSFeedUrl).FirstOrDefault();
            Assert.IsNotNull(feedInRepo);
            Assert.IsTrue(feedInRepo.RSSFeedTitle == "NewTitle");
        }

        #endregion

        #region NavigateToRSSFeedItems

        private ViewResult navigateToRSSFeedItemsListResult;
        private IEnumerable<RSSFeed> navigateToRSSFeedItemsFeeds;
        private RSSFeed navigateToRSSFeedItemsFeed;
        private ActionResult navigateToRSSFeedItemsResult;

        [Test]
        public void NavigateToRSSFeedItemsPage()
        {
            new Story("Navigate to RSS Feed Items Page")
                .InOrderTo("have a more detailed view of RSS Feed items")
                .AsA("User")
                .IWant("to be redirected to a page where a list of RSS Feed items are displayed")

                        .WithScenario("User Clicks on RSS Feed Items")
                            .Given(TheUserIsOnTheFeedPage)
                            .When(TheViewRSSFeedItemsButtonIsClicked)
                            .Then(TheUserShouldBeRedirectedToAPageWhereTheRSSFeedItemsAreDisplayed)
                .Execute();
        }
        private void TheUserIsOnTheFeedPage()
        {
            Assert.IsNotNull(RSSFeedPage, "The Rss feed page must be defined");
            Assert.IsNotNull(RSSFeedPage.User.Identity.Name);
        }
        private void TheViewRSSFeedItemsButtonIsClicked()
        {
            navigateToRSSFeedItemsListResult = RSSFeedPage.List() as ViewResult;
            Assert.IsNotNull(navigateToRSSFeedItemsListResult, "The list of feeds could not be retrieved");

            navigateToRSSFeedItemsFeeds = (navigateToRSSFeedItemsListResult.Model as IEnumerable<RSSFeed>);
            Assert.IsNotNull(navigateToRSSFeedItemsFeeds);

            this.rssFeedRssFeedToBeEdited = navigateToRSSFeedItemsFeeds.FirstOrDefault();
            Assert.IsNotNull(this.rssFeedRssFeedToBeEdited);

            navigateToRSSFeedItemsResult = RSSFeedPage.NavigateToRSSFeedItems(this.rssFeedRssFeedToBeEdited);
        }
        private void TheUserShouldBeRedirectedToAPageWhereTheRSSFeedItemsAreDisplayed()
        {
            var redirectResult = (navigateToRSSFeedItemsResult as RedirectToRouteResult);
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(redirectResult.RouteValues["rssFeedUrl"], this.rssFeedRssFeedToBeEdited.RSSFeedUrl);
            Assert.AreEqual(redirectResult.RouteValues["controller"], "RSSFeedItems");
            Assert.AreEqual(redirectResult.RouteValues["action"], "List");
        }

        #endregion
    }
}

