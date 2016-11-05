namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduledModels", "DayMonday", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledModels", "DayTuesday", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledModels", "DayWednsday", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledModels", "DayThursday", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledModels", "DayFriday", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledModels", "DaySaturday", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledModels", "DaySunday", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledModels", "Hour", c => c.Int(nullable: false));
            AddColumn("dbo.ScheduledModels", "Minute", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduledModels", "Minute");
            DropColumn("dbo.ScheduledModels", "Hour");
            DropColumn("dbo.ScheduledModels", "DaySunday");
            DropColumn("dbo.ScheduledModels", "DaySaturday");
            DropColumn("dbo.ScheduledModels", "DayFriday");
            DropColumn("dbo.ScheduledModels", "DayThursday");
            DropColumn("dbo.ScheduledModels", "DayWednsday");
            DropColumn("dbo.ScheduledModels", "DayTuesday");
            DropColumn("dbo.ScheduledModels", "DayMonday");
        }
    }
}
