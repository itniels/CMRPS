namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m15 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ComputerModels", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ComputerModels", "Name", c => c.String(nullable: false));
        }
    }
}
