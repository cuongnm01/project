namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User_Product")]
    public partial class User_Product
    {
        public Guid User_ProductID { get; set; }
        public Guid? UserID { get; set; }
        public Guid ProductId { get; set; }
        public int? StatusID { get; set; }
    }
}
