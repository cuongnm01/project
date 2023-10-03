namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCategoryTbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categorys", "IsHomePage", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categorys", "IsHomePage");
        }
    }
}
