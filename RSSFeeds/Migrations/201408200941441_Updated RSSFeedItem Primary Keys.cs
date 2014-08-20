namespace RSSFeeds.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedRSSFeedItemPrimaryKeys : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.RSSFeedItems");
            AlterColumn("dbo.RSSFeedItems", "Title", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.RSSFeedItems", new[] { "RSSFeedItemId", "Title" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.RSSFeedItems");
            AlterColumn("dbo.RSSFeedItems", "Title", c => c.String());
            AddPrimaryKey("dbo.RSSFeedItems", "RSSFeedItemId");
        }
    }
}
