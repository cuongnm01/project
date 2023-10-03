namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateHeaderIngredientAndDirection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductDirections", "ProductDirectionGroupId", c => c.Guid());
            AddColumn("dbo.ProductIngredients", "ProductIngredientGroupId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductIngredients", "ProductIngredientGroupId");
            DropColumn("dbo.ProductDirections", "ProductDirectionGroupId");
        }
    }
}
