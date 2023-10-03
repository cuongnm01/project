
namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class ProductIngredientGroup
    {
        public Guid ProductIngredientGroupId { get; set; }

        public Guid? ProductId { get; set; }
        [StringLength(500)]
        public string Name { get; set; }

        public int? SortOrder { get; set; }

        public int? StatusID { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
