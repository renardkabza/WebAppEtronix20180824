namespace WebAppEtronix20180824.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSubMenu")]
    public partial class tblSubMenu
    {
        public tblSubMenu()
        {
            tlbRoles = new HashSet<tlbRoles>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string SubMenu { get; set; }

        [StringLength(100)]
        public string Controller { get; set; }

        [StringLength(100)]
        public string Action { get; set; }

        public int? Position { get; set; }

        public int? tblMainMenuId { get; set; }

        public virtual tblMainMenu tblMainMenu { get; set; }

        public virtual ICollection<tlbRoles> tlbRoles { get; set; }
    }
}
