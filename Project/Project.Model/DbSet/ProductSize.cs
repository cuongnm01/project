namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductSize
    {
        public Guid ProductSizeId { get; set; }

        public Guid? ProductId { get; set; }

        public int? SizeId { get; set; }
        public int? StatusID { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
