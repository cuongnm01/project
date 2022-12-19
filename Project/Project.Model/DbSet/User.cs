namespace Project.Model.DbSet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        public Guid UserID { get; set; }

        [StringLength(50)]
        public string UserCode { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Avatar { get; set; }

        public string CoverImage { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public int? PermissionID { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        public int? GenderID { get; set; }

        public string Description { get; set; }

        public string FacebookKey { get; set; }

        public string GoogleKey { get; set; }

        public string AppleKey { get; set; }

        public int? StatusID { get; set; }

        [StringLength(2)]
        public string LanguageCode { get; set; }

        public string DeviceToken { get; set; }

        public DateTime? CreateDate { get; set; }

        public Guid? CreateBy { get; set; }

        [StringLength(500)]
        public string AccessToken { get; set; }

        public int? CompanyID { get; set; }

        public int? ManufacturyID { get; set; }

        public int? PositionID { get; set; }

        public DateTime? StartDateAT { get; set; }

        public DateTime? EndDateAT { get; set; }

        [StringLength(500)]
        public string Literacy { get; set; }

        public int? Experience { get; set; }

        [StringLength(500)]
        public string QrCode { get; set; }
    }
}
