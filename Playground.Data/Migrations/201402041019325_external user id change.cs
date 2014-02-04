namespace Playground.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class externaluseridchange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "ExternalUserID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "ExternalUserID", c => c.Int(nullable: false));
        }
    }
}
