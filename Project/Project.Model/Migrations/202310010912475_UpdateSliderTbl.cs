namespace Project.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSliderTbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sliders", "ProductId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sliders", "ProductId");
        }
    }
}
