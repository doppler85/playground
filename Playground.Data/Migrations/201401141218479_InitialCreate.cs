namespace Playground.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameCategories",
                c => new
                    {
                        GameCategoryID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        PictureUrl = c.String(),
                    })
                .PrimaryKey(t => t.GameCategoryID);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        GameID = c.Int(nullable: false, identity: true),
                        GameCategoryID = c.Int(nullable: false),
                        Title = c.String(),
                        PictureUrl = c.String(),
                    })
                .PrimaryKey(t => t.GameID)
                .ForeignKey("dbo.GameCategories", t => t.GameCategoryID, cascadeDelete: true)
                .Index(t => t.GameCategoryID);
            
            CreateTable(
                "dbo.GameCompetitors",
                c => new
                    {
                        GameID = c.Int(nullable: false),
                        CompetitorID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.GameID, t.CompetitorID })
                .ForeignKey("dbo.Games", t => t.GameID)
                .ForeignKey("dbo.Competitors", t => t.CompetitorID)
                .Index(t => t.GameID)
                .Index(t => t.CompetitorID);
            
            CreateTable(
                "dbo.CompetitorScores",
                c => new
                    {
                        CompetitorID = c.Long(nullable: false),
                        MatchID = c.Long(nullable: false),
                        Confirmed = c.Boolean(nullable: false),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.CompetitorID, t.MatchID })
                .ForeignKey("dbo.Competitors", t => t.CompetitorID)
                .ForeignKey("dbo.Matches", t => t.MatchID)
                .Index(t => t.CompetitorID)
                .Index(t => t.MatchID);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        MatchID = c.Long(nullable: false, identity: true),
                        CreatorID = c.Int(nullable: false),
                        GameID = c.Int(nullable: false),
                        CompetitionTypeID = c.Int(nullable: false),
                        WinnerID = c.Long(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MatchID)
                .ForeignKey("dbo.Users", t => t.CreatorID)
                .ForeignKey("dbo.Games", t => t.GameID, cascadeDelete: true)
                .ForeignKey("dbo.CompetitionTypes", t => t.CompetitionTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Competitors", t => t.WinnerID)
                .Index(t => t.CreatorID)
                .Index(t => t.GameID)
                .Index(t => t.CompetitionTypeID)
                .Index(t => t.WinnerID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Gender = c.Int(nullable: false),
                        EmailAddress = c.String(),
                        ExternalUserID = c.Int(nullable: false),
                        PictureUrl = c.String(),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Competitors",
                c => new
                    {
                        CompetitorID = c.Long(nullable: false, identity: true),
                        CompetitorType = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Name = c.String(),
                        Status = c.Int(nullable: false),
                        PictureUrl = c.String(),
                        UserID = c.Int(),
                        CreatorID = c.Int(),
                        CompetitorTypeDiscriminator = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompetitorID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatorID)
                .Index(t => t.UserID)
                .Index(t => t.CreatorID);
            
            CreateTable(
                "dbo.TeamPlayers",
                c => new
                    {
                        TeamID = c.Long(nullable: false),
                        PlayerID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.TeamID, t.PlayerID })
                .ForeignKey("dbo.Competitors", t => t.TeamID)
                .ForeignKey("dbo.Competitors", t => t.PlayerID)
                .Index(t => t.TeamID)
                .Index(t => t.PlayerID);
            
            CreateTable(
                "dbo.CompetitionTypes",
                c => new
                    {
                        CompetitionTypeID = c.Int(nullable: false, identity: true),
                        CompetitorType = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        CompetitorsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompetitionTypeID);
            
            CreateTable(
                "dbo.GameCompetitionTypes",
                c => new
                    {
                        GameID = c.Int(nullable: false),
                        CompetitionTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GameID, t.CompetitionTypeID })
                .ForeignKey("dbo.Games", t => t.GameID)
                .ForeignKey("dbo.CompetitionTypes", t => t.CompetitionTypeID)
                .Index(t => t.GameID)
                .Index(t => t.CompetitionTypeID);
            
            CreateTable(
                "dbo.AutomaticMatchConfirmations",
                c => new
                    {
                        ConfirmeeID = c.Int(nullable: false),
                        ConfirmerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ConfirmeeID, t.ConfirmerID })
                .ForeignKey("dbo.Users", t => t.ConfirmeeID)
                .ForeignKey("dbo.Users", t => t.ConfirmerID)
                .Index(t => t.ConfirmeeID)
                .Index(t => t.ConfirmerID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.AutomaticMatchConfirmations", new[] { "ConfirmerID" });
            DropIndex("dbo.AutomaticMatchConfirmations", new[] { "ConfirmeeID" });
            DropIndex("dbo.GameCompetitionTypes", new[] { "CompetitionTypeID" });
            DropIndex("dbo.GameCompetitionTypes", new[] { "GameID" });
            DropIndex("dbo.TeamPlayers", new[] { "PlayerID" });
            DropIndex("dbo.TeamPlayers", new[] { "TeamID" });
            DropIndex("dbo.Competitors", new[] { "CreatorID" });
            DropIndex("dbo.Competitors", new[] { "UserID" });
            DropIndex("dbo.Matches", new[] { "WinnerID" });
            DropIndex("dbo.Matches", new[] { "CompetitionTypeID" });
            DropIndex("dbo.Matches", new[] { "GameID" });
            DropIndex("dbo.Matches", new[] { "CreatorID" });
            DropIndex("dbo.CompetitorScores", new[] { "MatchID" });
            DropIndex("dbo.CompetitorScores", new[] { "CompetitorID" });
            DropIndex("dbo.GameCompetitors", new[] { "CompetitorID" });
            DropIndex("dbo.GameCompetitors", new[] { "GameID" });
            DropIndex("dbo.Games", new[] { "GameCategoryID" });
            DropForeignKey("dbo.AutomaticMatchConfirmations", "ConfirmerID", "dbo.Users");
            DropForeignKey("dbo.AutomaticMatchConfirmations", "ConfirmeeID", "dbo.Users");
            DropForeignKey("dbo.GameCompetitionTypes", "CompetitionTypeID", "dbo.CompetitionTypes");
            DropForeignKey("dbo.GameCompetitionTypes", "GameID", "dbo.Games");
            DropForeignKey("dbo.TeamPlayers", "PlayerID", "dbo.Competitors");
            DropForeignKey("dbo.TeamPlayers", "TeamID", "dbo.Competitors");
            DropForeignKey("dbo.Competitors", "CreatorID", "dbo.Users");
            DropForeignKey("dbo.Competitors", "UserID", "dbo.Users");
            DropForeignKey("dbo.Matches", "WinnerID", "dbo.Competitors");
            DropForeignKey("dbo.Matches", "CompetitionTypeID", "dbo.CompetitionTypes");
            DropForeignKey("dbo.Matches", "GameID", "dbo.Games");
            DropForeignKey("dbo.Matches", "CreatorID", "dbo.Users");
            DropForeignKey("dbo.CompetitorScores", "MatchID", "dbo.Matches");
            DropForeignKey("dbo.CompetitorScores", "CompetitorID", "dbo.Competitors");
            DropForeignKey("dbo.GameCompetitors", "CompetitorID", "dbo.Competitors");
            DropForeignKey("dbo.GameCompetitors", "GameID", "dbo.Games");
            DropForeignKey("dbo.Games", "GameCategoryID", "dbo.GameCategories");
            DropTable("dbo.AutomaticMatchConfirmations");
            DropTable("dbo.GameCompetitionTypes");
            DropTable("dbo.CompetitionTypes");
            DropTable("dbo.TeamPlayers");
            DropTable("dbo.Competitors");
            DropTable("dbo.Users");
            DropTable("dbo.Matches");
            DropTable("dbo.CompetitorScores");
            DropTable("dbo.GameCompetitors");
            DropTable("dbo.Games");
            DropTable("dbo.GameCategories");
        }
    }
}
