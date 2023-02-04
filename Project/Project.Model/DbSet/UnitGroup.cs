namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UnitGroup")]
    public partial class UnitGroup
    {
        public int UnitGroupId { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public int? StatusID { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
