using Project.Model.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.Model
{
    public class ProductInfo
    {
        public Product Product { get; set; }
        public Category Category { get; set; }
    }
    public class ProductIngredientInfo
    {
        public ProductIngredient ProductIngredient { get; set; }
        public Ingredient Ingredient { get; set; }
    }

    public class UnitGroupInfo
    {
        public UnitGroup UnitGroup { get; set; }
        public int TotalUnit { get; set; }
        public int TotalIngredient { get; set; }
    }

    public class UnitInfo
    {
        public Unit Unit { get; set; }
        public UnitGroup UnitGroup { get; set; }

    }

    public class IngredientGroupInfo
    {
        public IngredientGroup IngredientGroup { get; set; }
        public int TotalIngredient { get; set; }
    }
    public class IngredientInfo
    {
        public Ingredient Ingredient { get; set; }
        public IngredientGroup IngredientGroup { get; set; }
        public UnitGroup UnitGroup { get; set; }

    }

    public class CategoryInfo
    {
        public Category Category { get; set; }
        public int TotalRecipe { get; set; }
    }
}
