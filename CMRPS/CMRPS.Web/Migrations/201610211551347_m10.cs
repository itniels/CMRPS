namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m10 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ComputerModels", "Color_Id", "dbo.ColorModels");
            DropForeignKey("dbo.ComputerModels", "Location_Id", "dbo.LocationModels");
            DropForeignKey("dbo.ComputerModels", "Type_Id", "dbo.ComputerTypeModels");
            DropIndex("dbo.ComputerModels", new[] { "Color_Id" });
            DropIndex("dbo.ComputerModels", new[] { "Location_Id" });
            DropIndex("dbo.ComputerModels", new[] { "Type_Id" });
            AlterColumn("dbo.ComputerModels", "Color_Id", c => c.Int());
            AlterColumn("dbo.ComputerModels", "Location_Id", c => c.Int());
            AlterColumn("dbo.ComputerModels", "Type_Id", c => c.Int());
            CreateIndex("dbo.ComputerModels", "Color_Id");
            CreateIndex("dbo.ComputerModels", "Location_Id");
            CreateIndex("dbo.ComputerModels", "Type_Id");
            AddForeignKey("dbo.ComputerModels", "Color_Id", "dbo.ColorModels", "Id");
            AddForeignKey("dbo.ComputerModels", "Location_Id", "dbo.LocationModels", "Id");
            AddForeignKey("dbo.ComputerModels", "Type_Id", "dbo.ComputerTypeModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ComputerModels", "Type_Id", "dbo.ComputerTypeModels");
            DropForeignKey("dbo.ComputerModels", "Location_Id", "dbo.LocationModels");
            DropForeignKey("dbo.ComputerModels", "Color_Id", "dbo.ColorModels");
            DropIndex("dbo.ComputerModels", new[] { "Type_Id" });
            DropIndex("dbo.ComputerModels", new[] { "Location_Id" });
            DropIndex("dbo.ComputerModels", new[] { "Color_Id" });
            AlterColumn("dbo.ComputerModels", "Type_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.ComputerModels", "Location_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.ComputerModels", "Color_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ComputerModels", "Type_Id");
            CreateIndex("dbo.ComputerModels", "Location_Id");
            CreateIndex("dbo.ComputerModels", "Color_Id");
            AddForeignKey("dbo.ComputerModels", "Type_Id", "dbo.ComputerTypeModels", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ComputerModels", "Location_Id", "dbo.LocationModels", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ComputerModels", "Color_Id", "dbo.ColorModels", "Id", cascadeDelete: true);
        }
    }
}
