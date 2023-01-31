﻿namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Unit")]
    public partial class Unit
    {
        public int UnitId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        public bool? IsDefault { get; set; }
        public double? Rate { get; set; }
        public int? SortOrder { get; set; }
        public int? StatusID { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
