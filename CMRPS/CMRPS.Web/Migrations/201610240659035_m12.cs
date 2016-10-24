namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SiteSettings", "PingInterval", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SiteSettings", "PingInterval");
        }
    }
}
