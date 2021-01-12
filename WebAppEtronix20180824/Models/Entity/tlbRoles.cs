namespace WebAppEtronix20180824.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tlbRoles
    {
        public int Id { get; set; }

        public int tlbSubMenuId { get; set; }

        [StringLength(128)]
        public string AspNetUsersId { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual tblSubMenu tblSubMenu { get; set; }
    }
}
