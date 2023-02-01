namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPermissionTbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User_Permission",
                c => new
                    {
                        User_PermissionID = c.Guid(nullable: false),
                        UserID = c.Guid(),
                        Functions = c.Int(),
                        Fulls = c.Int(),
                        Views = c.Int(),
                        Updates = c.Int(),
                        Deletes = c.Int(),
                    })
                .PrimaryKey(t => t.User_PermissionID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.User_Permission");
        }
    }
}
