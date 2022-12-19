namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Token")]
    public partial class Token
    {
        public Guid TokenID { get; set; }

        public Guid? UserID { get; set; }

        public string TokenKey { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }
}
