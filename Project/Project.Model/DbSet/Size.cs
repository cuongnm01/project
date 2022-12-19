namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Size
    {
        public int SizeId { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public int? SortOrder { get; set; }
    }
}
