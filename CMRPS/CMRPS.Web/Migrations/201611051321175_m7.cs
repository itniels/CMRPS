namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m7 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ScheduledModels", newName: "Schedules");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Schedules", newName: "ScheduledModels");
        }
    }
}
