namespace RSSFeeds.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReadFlagonRSSFeedItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RSSFeedItems", "Read", c => c.Boolean(nullable: false));
            AddForeignKey("dbo.RSSFeedItems", "UserId", "dbo.UserProfile", "UserId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RSSFeedItems", "UserId", "dbo.UserProfile");
            DropColumn("dbo.RSSFeedItems", "Read");
        }
    }
}
