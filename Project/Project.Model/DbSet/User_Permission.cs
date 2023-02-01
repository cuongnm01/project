namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User_Permission")]
    public partial class User_Permission
    {
        public Guid User_PermissionID { get; set; }
        public Guid? UserID { get; set; }
        public int? Functions { get; set; }
        public int? Fulls { get; set; }
        public int? Views { get; set; }
        public int? Updates { get; set; }
        public int? Deletes { get; set; }
    }
}
