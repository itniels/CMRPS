namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.InfoModels", "CPUCores", c => c.String());
            AlterColumn("dbo.InfoModels", "RAMSize", c => c.String());
            AlterColumn("dbo.InfoModels", "DiskSize", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InfoModels", "DiskSize", c => c.Int(nullable: false));
            AlterColumn("dbo.InfoModels", "RAMSize", c => c.Int(nullable: false));
            AlterColumn("dbo.InfoModels", "CPUCores", c => c.Int(nullable: false));
        }
    }
}
