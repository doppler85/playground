namespace Playground.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlayersPerTeam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompetitionTypes", "PlayersPerTeam", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompetitionTypes", "PlayersPerTeam");
        }
    }
}
