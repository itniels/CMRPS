namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m11 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ComputerModels", "info_Id", "dbo.InfoModels");
            DropIndex("dbo.ComputerModels", new[] { "info_Id" });
            AddColumn("dbo.ComputerModels", "PurchaseDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ComputerModels", "Description", c => c.String());
            AddColumn("dbo.ComputerModels", "Manufacturer", c => c.String());
            AddColumn("dbo.ComputerModels", "Model", c => c.String());
            AddColumn("dbo.ComputerModels", "CPU", c => c.String());
            AddColumn("dbo.ComputerModels", "CPUCores", c => c.String());
            AddColumn("dbo.ComputerModels", "RAM", c => c.String());
            AddColumn("dbo.ComputerModels", "RAMSize", c => c.String());
            AddColumn("dbo.ComputerModels", "Disk", c => c.String());
            AddColumn("dbo.ComputerModels", "DiskSize", c => c.String());
            AddColumn("dbo.ComputerModels", "EthernetCable", c => c.String());
            AddColumn("dbo.ComputerModels", "EthernetWifi", c => c.String());
            AddColumn("dbo.ComputerModels", "OS", c => c.String());
            DropColumn("dbo.ComputerModels", "info_Id");
            DropTable("dbo.InfoModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.InfoModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        PurchaseDate = c.DateTime(nullable: false),
                        Manufacturer = c.String(),
                        Model = c.String(),
                        CPU = c.String(),
                        CPUCores = c.String(),
                        RAM = c.String(),
                        RAMSize = c.String(),
                        Disk = c.String(),
                        DiskSize = c.String(),
                        EthernetCable = c.String(),
                        EthernetWifi = c.String(),
                        OS = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ComputerModels", "info_Id", c => c.Int());
            DropColumn("dbo.ComputerModels", "OS");
            DropColumn("dbo.ComputerModels", "EthernetWifi");
            DropColumn("dbo.ComputerModels", "EthernetCable");
            DropColumn("dbo.ComputerModels", "DiskSize");
            DropColumn("dbo.ComputerModels", "Disk");
            DropColumn("dbo.ComputerModels", "RAMSize");
            DropColumn("dbo.ComputerModels", "RAM");
            DropColumn("dbo.ComputerModels", "CPUCores");
            DropColumn("dbo.ComputerModels", "CPU");
            DropColumn("dbo.ComputerModels", "Model");
            DropColumn("dbo.ComputerModels", "Manufacturer");
            DropColumn("dbo.ComputerModels", "Description");
            DropColumn("dbo.ComputerModels", "PurchaseDate");
            CreateIndex("dbo.ComputerModels", "info_Id");
            AddForeignKey("dbo.ComputerModels", "info_Id", "dbo.InfoModels", "Id");
        }
    }
}
