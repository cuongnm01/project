namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AppConfig")]
    public partial class AppConfig
    {
        public Guid AppConfigID { get; set; }

        public string Logo { get; set; }

        public string ServerKey { get; set; }

        public string SenderId { get; set; }

        [StringLength(500)]
        public string GoogleAPIKey { get; set; }

        public Guid? AccessKey { get; set; }

        public double? VersionMaster { get; set; }

        public int? RatingBonus { get; set; }

        public int? RatingPoints { get; set; }

        public int? ViewBanner { get; set; }

        public string Video { get; set; }

        public string VideoThumb { get; set; }

        public string Policy { get; set; }

        public string Policy_En { get; set; }

        public string Tutorial { get; set; }

        [StringLength(255)]
        public string Phone { get; set; }

        [StringLength(255)]
        public string Zalo { get; set; }

        [StringLength(255)]
        public string FacebookID { get; set; }

        public string Contact { get; set; }

        public string Contact_En { get; set; }

        public string Terms_Of_Use { get; set; }

        public string QRCodeDescription { get; set; }
    }
}
