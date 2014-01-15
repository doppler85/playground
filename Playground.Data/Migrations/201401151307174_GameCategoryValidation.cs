namespace Playground.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameCategoryValidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GameCategories", "Title", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.CompetitionTypes", "Name", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompetitionTypes", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.GameCategories", "Title", c => c.String());
        }
    }
}
