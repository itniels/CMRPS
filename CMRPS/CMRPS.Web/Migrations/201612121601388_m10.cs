namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Computers", "IsOnline", c => c.Boolean(nullable: false));
            DropColumn("dbo.Computers", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Computers", "Status", c => c.Boolean(nullable: false));
            DropColumn("dbo.Computers", "IsOnline");
        }
    }
}
