namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Ingredient
    {
        public int IngredientId { get; set; }
        public int IngredientGroupId { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public int? SortOrder { get; set; }
        public int? StatusID { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UnitGroupId { get; set; }
        public double? Price { get; set; }

    }
}
