namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User_Category")]
    public partial class User_Category
    {
        public Guid User_CategoryID { get; set; }
        public Guid? UserID { get; set; }
        public int? CategoryId { get; set; }
    }
}
