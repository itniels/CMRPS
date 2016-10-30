namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m5 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ColorModels", newName: "Colors");
            RenameTable(name: "dbo.ComputerModels", newName: "Computers");
            RenameTable(name: "dbo.LocationModels", newName: "Locations");
            RenameTable(name: "dbo.ComputerTypeModels", newName: "ComputerTypes");
            AddColumn("dbo.Computers", "isBusy", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Computers", "isBusy");
            RenameTable(name: "dbo.ComputerTypes", newName: "ComputerTypeModels");
            RenameTable(name: "dbo.Locations", newName: "LocationModels");
            RenameTable(name: "dbo.Computers", newName: "ComputerModels");
            RenameTable(name: "dbo.Colors", newName: "ColorModels");
        }
    }
}
