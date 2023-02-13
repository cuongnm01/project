namespace Project.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Project.Model.DbSet;

    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=AppDbContext")
        {
        }

        public virtual DbSet<AppConfig> AppConfigs { get; set; }
        public virtual DbSet<Category> Categorys { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<IngredientGroup> IngredientGroups { get; set; }
        public virtual DbSet<ProductDirection> ProductDirections { get; set; }
        public virtual DbSet<ProductIngredient> ProductIngredients { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductSize> ProductSizes { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<UnitGroup> UnitGroups { get; set; }

        public virtual DbSet<User_Permission> User_Permission { get; set; }
        public virtual DbSet<User_Category> User_Category { get; set; }
        public virtual DbSet<User_Product> User_Product { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<District>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<User>()
                .Property(e => e.LanguageCode)
                .IsUnicode(false);
        }
    }
}
