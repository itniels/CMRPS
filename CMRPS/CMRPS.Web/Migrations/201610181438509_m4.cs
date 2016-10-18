namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ColorModels", "Color", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ColorModels", "Color", c => c.Int(nullable: false));
        }
    }
}
