namespace RSSFeeds.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSolution : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RSSFeedItems",
                c => new
                    {
                        RSSFeedItemId = c.String(nullable: false, maxLength: 128),
                        Title = c.String(),
                        Summary = c.String(),
                        UserId = c.Int(nullable: false),
                        RSSFeedUrl = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.RSSFeedItemId)
                .ForeignKey("dbo.RSSFeeds", t => new { t.RSSFeedUrl, t.UserId })
                .Index(t => new { t.RSSFeedUrl, t.UserId });
            
            CreateTable(
                "dbo.RSSFeeds",
                c => new
                    {
                        RSSFeedUrl = c.String(nullable: false, maxLength: 255),
                        UserId = c.Int(nullable: false),
                        RSSFeedTitle = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new { t.RSSFeedUrl, t.UserId })
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RSSFeeds", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.RSSFeedItems", new[] { "RSSFeedUrl", "UserId" }, "dbo.RSSFeeds");
            DropIndex("dbo.RSSFeeds", new[] { "UserId" });
            DropIndex("dbo.RSSFeedItems", new[] { "RSSFeedUrl", "UserId" });
            DropTable("dbo.UserProfile");
            DropTable("dbo.RSSFeeds");
            DropTable("dbo.RSSFeedItems");
        }
    }
}
