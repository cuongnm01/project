namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Slider
    {
        public int SliderId { get; set; }

        [StringLength(500)]
        public string Url { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        public int? SortOrder { get; set; }
    }
}
