namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Colors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ColorLabel = c.String(),
                        ColorText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Computers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IP = c.String(),
                        MAC = c.String(),
                        Hostname = c.String(nullable: false),
                        Status = c.Boolean(nullable: false),
                        PurchaseDate = c.DateTime(nullable: false),
                        Description = c.String(),
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
                        Color_Id = c.Int(),
                        Location_Id = c.Int(),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Colors", t => t.Color_Id)
                .ForeignKey("dbo.Locations", t => t.Location_Id)
                .ForeignKey("dbo.ComputerTypes", t => t.Type_Id)
                .Index(t => t.Color_Id)
                .Index(t => t.Location_Id)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ComputerTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ImagePath = c.String(),
                        Filename = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        Action = c.Int(nullable: false),
                        ActionStatus = c.Int(nullable: false),
                        Name = c.String(),
                        Username = c.String(),
                        Description = c.String(),
                        Exception = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Logins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        Name = c.String(),
                        Username = c.String(),
                        Success = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
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
                        PingInterval = c.Int(nullable: false),
                        CleanLogs = c.Boolean(nullable: false),
                        KeepLogsFor = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Logins", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Computers", "Type_Id", "dbo.ComputerTypes");
            DropForeignKey("dbo.Computers", "Location_Id", "dbo.Locations");
            DropForeignKey("dbo.Computers", "Color_Id", "dbo.Colors");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Logins", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Events", new[] { "User_Id" });
            DropIndex("dbo.Computers", new[] { "Type_Id" });
            DropIndex("dbo.Computers", new[] { "Location_Id" });
            DropIndex("dbo.Computers", new[] { "Color_Id" });
            DropTable("dbo.SiteSettings");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Logins");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Events");
            DropTable("dbo.ComputerTypes");
            DropTable("dbo.Locations");
            DropTable("dbo.Computers");
            DropTable("dbo.Colors");
        }
    }
}
