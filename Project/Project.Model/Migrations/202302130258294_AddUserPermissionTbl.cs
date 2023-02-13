namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserPermissionTbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User_Category",
                c => new
                    {
                        User_CategoryID = c.Guid(nullable: false),
                        UserID = c.Guid(),
                        CategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.User_CategoryID);
            
            CreateTable(
                "dbo.User_Product",
                c => new
                    {
                        User_ProductID = c.Guid(nullable: false),
                        UserID = c.Guid(),
                        ProductId = c.Guid(nullable: false),
                        StatusID = c.Int(),
                    })
                .PrimaryKey(t => t.User_ProductID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.User_Product");
            DropTable("dbo.User_Category");
        }
    }
}
