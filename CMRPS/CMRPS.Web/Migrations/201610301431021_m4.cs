namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SiteSettings", "CleanLogs", c => c.Boolean(nullable: false));
            AddColumn("dbo.SiteSettings", "KeepLogsFor", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SiteSettings", "KeepLogsFor");
            DropColumn("dbo.SiteSettings", "CleanLogs");
        }
    }
}
