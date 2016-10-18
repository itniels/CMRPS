namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AdminUsername = c.String(),
                        AdminPassword = c.String(),
                        AdminDomain = c.String(),
                        ShutdownMethod = c.Int(nullable: false),
                        ShutdownForce = c.Boolean(nullable: false),
                        ShutdownTimeout = c.Int(nullable: false),
                        ShutdownMessage = c.String(),
                        RebootMethod = c.Int(nullable: false),
                        RebootForce = c.Boolean(nullable: false),
                        RebootTimeout = c.Int(nullable: false),
                        RebootMessage = c.String(),
                        StartupMethod = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SiteSettings");
        }
    }
}
