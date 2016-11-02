namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SiteSettings", "WorkerQueues", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SiteSettings", "WorkerQueues");
        }
    }
}
