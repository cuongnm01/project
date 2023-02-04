namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateGroupTbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IngredientGroups",
                c => new
                    {
                        IngredientGroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.IngredientGroupId);
            
            CreateTable(
                "dbo.UnitGroup",
                c => new
                    {
                        UnitGroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UnitGroupId);
            
            AddColumn("dbo.Ingredients", "IngredientGroupId", c => c.Int(nullable: false));
            AddColumn("dbo.Ingredients", "UnitGroupId", c => c.Int());
            AddColumn("dbo.Ingredients", "Price", c => c.Double());
            AddColumn("dbo.ProductIngredients", "UnitId", c => c.Int());
            AddColumn("dbo.ProductIngredients", "Price", c => c.Double());
            AddColumn("dbo.Unit", "UnitGroupId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Unit", "UnitGroupId");
            DropColumn("dbo.ProductIngredients", "Price");
            DropColumn("dbo.ProductIngredients", "UnitId");
            DropColumn("dbo.Ingredients", "Price");
            DropColumn("dbo.Ingredients", "UnitGroupId");
            DropColumn("dbo.Ingredients", "IngredientGroupId");
            DropTable("dbo.UnitGroup");
            DropTable("dbo.IngredientGroups");
        }
    }
}
