namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ColorModels", "ColorLabel", c => c.String());
            AddColumn("dbo.ColorModels", "ColorText", c => c.String());
            DropColumn("dbo.ColorModels", "Color");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ColorModels", "Color", c => c.String());
            DropColumn("dbo.ColorModels", "ColorText");
            DropColumn("dbo.ColorModels", "ColorLabel");
        }
    }
}
