namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m14 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ComputerTypeModels", "ImagePath", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ComputerTypeModels", "ImagePath", c => c.String(nullable: false));
        }
    }
}
