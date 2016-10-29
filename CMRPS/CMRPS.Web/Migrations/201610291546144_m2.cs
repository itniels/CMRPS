namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Name", c => c.String());
            AddColumn("dbo.Events", "Username", c => c.String());
            AddColumn("dbo.Logins", "Name", c => c.String());
            AddColumn("dbo.Logins", "Username", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logins", "Username");
            DropColumn("dbo.Logins", "Name");
            DropColumn("dbo.Events", "Username");
            DropColumn("dbo.Events", "Name");
        }
    }
}
