namespace App.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_sample_model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.Movies",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Category = c.String(),
                        Language = c.String(),
                        ReleaseDate = c.DateTime(),
                        Rating = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.ShowTimes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(),
                        Price = c.Decimal(precision: 21, scale: 3),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                        Movie_Id = c.String(maxLength: 128),
                        Theatre_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Movies", t => t.Movie_Id)
                .ForeignKey("public.Theatres", t => t.Theatre_Id)
                .Index(t => t.Movie_Id)
                .Index(t => t.Theatre_Id);
            
            CreateTable(
                "public.Theatres",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        NumberOfSeat = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.Tickets",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PaymentMethod = c.String(),
                        OriginalPrice = c.Decimal(precision: 21, scale: 3),
                        PromoCode = c.String(),
                        Price = c.Decimal(precision: 21, scale: 3),
                        Discount = c.Decimal(precision: 21, scale: 3),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsDraft = c.Boolean(nullable: false),
                        ShowTime_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.ShowTimes", t => t.ShowTime_Id)
                .Index(t => t.ShowTime_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("public.Tickets", "ShowTime_Id", "public.ShowTimes");
            DropForeignKey("public.ShowTimes", "Theatre_Id", "public.Theatres");
            DropForeignKey("public.ShowTimes", "Movie_Id", "public.Movies");
            DropIndex("public.Tickets", new[] { "ShowTime_Id" });
            DropIndex("public.ShowTimes", new[] { "Theatre_Id" });
            DropIndex("public.ShowTimes", new[] { "Movie_Id" });
            DropTable("public.Tickets");
            DropTable("public.Theatres");
            DropTable("public.ShowTimes");
            DropTable("public.Movies");
        }
    }
}
