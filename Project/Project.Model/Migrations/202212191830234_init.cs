namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppConfig",
                c => new
                    {
                        AppConfigID = c.Guid(nullable: false),
                        Logo = c.String(),
                        ServerKey = c.String(),
                        SenderId = c.String(),
                        GoogleAPIKey = c.String(maxLength: 500),
                        AccessKey = c.Guid(),
                        VersionMaster = c.Double(),
                        RatingBonus = c.Int(),
                        RatingPoints = c.Int(),
                        ViewBanner = c.Int(),
                        Video = c.String(),
                        VideoThumb = c.String(),
                        Policy = c.String(),
                        Policy_En = c.String(),
                        Tutorial = c.String(),
                        Phone = c.String(maxLength: 255),
                        Zalo = c.String(maxLength: 255),
                        FacebookID = c.String(maxLength: 255),
                        Contact = c.String(),
                        Contact_En = c.String(),
                        Terms_Of_Use = c.String(),
                        QRCodeDescription = c.String(),
                    })
                .PrimaryKey(t => t.AppConfigID);
            
            CreateTable(
                "dbo.Categorys",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 50),
                        Name = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.District",
                c => new
                    {
                        DistrictID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250),
                        Type = c.String(maxLength: 50),
                        ProvinceID = c.Int(nullable: false),
                        Price = c.Decimal(precision: 18, scale: 0),
                    })
                .PrimaryKey(t => t.DistrictID);
            
            CreateTable(
                "dbo.Ingredients",
                c => new
                    {
                        IngredientId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.IngredientId);
            
            CreateTable(
                "dbo.ProductDirections",
                c => new
                    {
                        ProductDirectionId = c.Guid(nullable: false),
                        ProductId = c.Guid(),
                        Code = c.String(maxLength: 50),
                        Name = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        Description = c.String(),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductDirectionId);
            
            CreateTable(
                "dbo.ProductIngredients",
                c => new
                    {
                        ProductIngredientId = c.Guid(nullable: false),
                        ProductId = c.Guid(),
                        SizeId = c.Int(),
                        IngredientId = c.Int(),
                        Value = c.Double(),
                        Unit = c.String(maxLength: 50),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductIngredientId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Guid(nullable: false),
                        Code = c.String(maxLength: 50),
                        CategoryId = c.Int(),
                        Name = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        Background = c.String(maxLength: 500),
                        IsNew = c.Int(),
                        VideoUrl = c.String(maxLength: 500),
                        VideoTitle = c.String(maxLength: 500),
                        VideoDescription = c.String(maxLength: 500),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductSizes",
                c => new
                    {
                        ProductSizeId = c.Guid(nullable: false),
                        ProductId = c.Guid(),
                        SizeId = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductSizeId);
            
            CreateTable(
                "dbo.Provinces",
                c => new
                    {
                        ProvinceID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Type = c.String(maxLength: 20),
                        CountryID = c.Int(),
                        Code = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.ProvinceID);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        SizeId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SizeId);
            
            CreateTable(
                "dbo.Sliders",
                c => new
                    {
                        SliderId = c.Int(nullable: false, identity: true),
                        Url = c.String(maxLength: 500),
                        Title = c.String(maxLength: 500),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SliderId);
            
            CreateTable(
                "dbo.Token",
                c => new
                    {
                        TokenID = c.Guid(nullable: false),
                        UserID = c.Guid(),
                        TokenKey = c.String(),
                        CreateDate = c.DateTime(),
                        ExpirationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.TokenID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Guid(nullable: false),
                        UserCode = c.String(maxLength: 50),
                        UserName = c.String(maxLength: 100),
                        Password = c.String(),
                        Avatar = c.String(),
                        CoverImage = c.String(),
                        FullName = c.String(maxLength: 100),
                        DateOfBirth = c.DateTime(),
                        Phone = c.String(maxLength: 20),
                        Email = c.String(maxLength: 100),
                        PermissionID = c.Int(),
                        Address = c.String(maxLength: 500),
                        GenderID = c.Int(),
                        Description = c.String(),
                        FacebookKey = c.String(),
                        GoogleKey = c.String(),
                        AppleKey = c.String(),
                        StatusID = c.Int(),
                        LanguageCode = c.String(maxLength: 2, unicode: false),
                        DeviceToken = c.String(),
                        CreateDate = c.DateTime(),
                        CreateBy = c.Guid(),
                        AccessToken = c.String(maxLength: 500),
                        CompanyID = c.Int(),
                        ManufacturyID = c.Int(),
                        PositionID = c.Int(),
                        StartDateAT = c.DateTime(),
                        EndDateAT = c.DateTime(),
                        Literacy = c.String(maxLength: 500),
                        Experience = c.Int(),
                        QrCode = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Ward",
                c => new
                    {
                        WardID = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        Type = c.String(maxLength: 50),
                        DistrictID = c.Int(),
                    })
                .PrimaryKey(t => t.WardID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Ward");
            DropTable("dbo.Users");
            DropTable("dbo.Token");
            DropTable("dbo.Sliders");
            DropTable("dbo.Sizes");
            DropTable("dbo.Provinces");
            DropTable("dbo.ProductSizes");
            DropTable("dbo.Products");
            DropTable("dbo.ProductIngredients");
            DropTable("dbo.ProductDirections");
            DropTable("dbo.Ingredients");
            DropTable("dbo.District");
            DropTable("dbo.Categorys");
            DropTable("dbo.AppConfig");
        }
    }
}
