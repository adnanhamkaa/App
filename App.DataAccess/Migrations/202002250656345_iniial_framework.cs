namespace App.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class iniial_framework : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.ActionAuthorizations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ActionName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "public.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("public.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("public.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "public.ActivityLogs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IPAddress = c.String(),
                        HostName = c.String(),
                        Username = c.String(),
                        Modul = c.String(),
                        Action = c.String(),
                        Url = c.String(),
                        Data = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.Flows",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Order = c.Int(nullable: false),
                        Keterangan = c.String(),
                        Url = c.String(),
                        Status = c.String(),
                        Roles = c.String(),
                        Data1 = c.String(),
                        Data2 = c.String(),
                        DueDate = c.DateTime(),
                        EmailGuid = c.String(),
                        EmailSequence = c.Int(),
                        MailSent = c.Boolean(),
                        RoleRecipient = c.String(),
                        Recipients = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        IsSendEmail = c.Boolean(),
                        Type = c.String(),
                        DisplayGrouping = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                        Workflow_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Workflows", t => t.Workflow_Id)
                .Index(t => t.Workflow_Id);
            
            CreateTable(
                "public.KeyValueStores",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Key = c.String(),
                        Value = c.String(),
                        Value2 = c.String(),
                        Desc = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.SynchronizerLogs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Action = c.String(),
                        Status = c.String(),
                        Data = c.String(),
                        ExtSystemName = c.String(),
                        Message = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        Title = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
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
                "public.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "public.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("public.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "public.Workflows",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Type = c.String(),
                        DataId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.ApplicationRoleActionAuthorizations",
                c => new
                    {
                        ApplicationRole_Id = c.String(nullable: false, maxLength: 128),
                        ActionAuthorization_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationRole_Id, t.ActionAuthorization_Id })
                .ForeignKey("public.AspNetRoles", t => t.ApplicationRole_Id, cascadeDelete: true)
                .ForeignKey("public.ActionAuthorizations", t => t.ActionAuthorization_Id, cascadeDelete: true)
                .Index(t => t.ApplicationRole_Id)
                .Index(t => t.ActionAuthorization_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("public.Flows", "Workflow_Id", "public.Workflows");
            DropForeignKey("public.AspNetUserRoles", "UserId", "public.AspNetUsers");
            DropForeignKey("public.AspNetUserLogins", "UserId", "public.AspNetUsers");
            DropForeignKey("public.AspNetUserClaims", "UserId", "public.AspNetUsers");
            DropForeignKey("public.AspNetUserRoles", "RoleId", "public.AspNetRoles");
            DropForeignKey("public.ApplicationRoleActionAuthorizations", "ActionAuthorization_Id", "public.ActionAuthorizations");
            DropForeignKey("public.ApplicationRoleActionAuthorizations", "ApplicationRole_Id", "public.AspNetRoles");
            DropIndex("public.ApplicationRoleActionAuthorizations", new[] { "ActionAuthorization_Id" });
            DropIndex("public.ApplicationRoleActionAuthorizations", new[] { "ApplicationRole_Id" });
            DropIndex("public.AspNetUserLogins", new[] { "UserId" });
            DropIndex("public.AspNetUserClaims", new[] { "UserId" });
            DropIndex("public.AspNetUsers", "UserNameIndex");
            DropIndex("public.Flows", new[] { "Workflow_Id" });
            DropIndex("public.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("public.AspNetUserRoles", new[] { "UserId" });
            DropIndex("public.AspNetRoles", "RoleNameIndex");
            DropTable("public.ApplicationRoleActionAuthorizations");
            DropTable("public.Workflows");
            DropTable("public.AspNetUserLogins");
            DropTable("public.AspNetUserClaims");
            DropTable("public.AspNetUsers");
            DropTable("public.SynchronizerLogs");
            DropTable("public.KeyValueStores");
            DropTable("public.Flows");
            DropTable("public.ActivityLogs");
            DropTable("public.AspNetUserRoles");
            DropTable("public.AspNetRoles");
            DropTable("public.ActionAuthorizations");
        }
    }
}
