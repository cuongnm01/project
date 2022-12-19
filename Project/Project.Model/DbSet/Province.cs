namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Province
    {
        public int ProvinceID { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Type { get; set; }

        public int? CountryID { get; set; }

        [StringLength(20)]
        public string Code { get; set; }
    }
}
