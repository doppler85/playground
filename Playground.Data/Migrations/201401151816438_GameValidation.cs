namespace Playground.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameValidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Games", "Title", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Games", "Title", c => c.String());
        }
    }
}
