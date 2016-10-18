namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ColorModels", "Name", c => c.String());
            AlterColumn("dbo.ColorModels", "Color", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ColorModels", "Color", c => c.String());
            DropColumn("dbo.ColorModels", "Name");
        }
    }
}
