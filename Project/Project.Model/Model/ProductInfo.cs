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
}
