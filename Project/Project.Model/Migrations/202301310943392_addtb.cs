namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Unit",
                c => new
                    {
                        UnitId = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 50),
                        Name = c.String(maxLength: 500),
                        IsDefault = c.Boolean(),
                        Rate = c.Double(),
                        SortOrder = c.Int(),
                        StatusID = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UnitId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Unit");
        }
    }
}
