namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProductDirectionGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductDirectionGroups",
                c => new
                    {
                        ProductDirectionGroupId = c.Guid(nullable: false),
                        ProductId = c.Guid(),
                        Name = c.String(maxLength: 500),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductDirectionGroupId);
            
            CreateTable(
                "dbo.ProductIngredientGroups",
                c => new
                    {
                        ProductIngredientGroupId = c.Guid(nullable: false),
                        ProductId = c.Guid(),
                        Name = c.String(maxLength: 500),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductIngredientGroupId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductIngredientGroups");
            DropTable("dbo.ProductDirectionGroups");
        }
    }
}
