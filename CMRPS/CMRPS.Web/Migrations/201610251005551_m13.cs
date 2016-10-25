namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ComputerTypeModels", "Filename", c => c.String());
            AlterColumn("dbo.ComputerTypeModels", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.ComputerTypeModels", "ImagePath", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ComputerTypeModels", "ImagePath", c => c.String());
            AlterColumn("dbo.ComputerTypeModels", "Name", c => c.String());
            DropColumn("dbo.ComputerTypeModels", "Filename");
        }
    }
}
