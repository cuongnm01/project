namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        public Guid ProductId { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public int? CategoryId { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [StringLength(500)]
        public string Background { get; set; }

        public int? IsNew { get; set; }

        [StringLength(500)]
        public string VideoUrl { get; set; }

        [StringLength(500)]
        public string VideoTitle { get; set; }

        [StringLength(500)]
        public string VideoDescription { get; set; }
        public int? StatusID { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
