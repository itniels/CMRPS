namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ComputerModels", "info_Id", "dbo.InfoModels");
            DropIndex("dbo.ComputerModels", new[] { "info_Id" });
            AlterColumn("dbo.ComputerModels", "info_Id", c => c.Int());
            CreateIndex("dbo.ComputerModels", "info_Id");
            AddForeignKey("dbo.ComputerModels", "info_Id", "dbo.InfoModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ComputerModels", "info_Id", "dbo.InfoModels");
            DropIndex("dbo.ComputerModels", new[] { "info_Id" });
            AlterColumn("dbo.ComputerModels", "info_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ComputerModels", "info_Id");
            AddForeignKey("dbo.ComputerModels", "info_Id", "dbo.InfoModels", "Id", cascadeDelete: true);
        }
    }
}
