namespace WebAppEtronix20180824.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblMainMenu")]
    public partial class tblMainMenu
    {
        public tblMainMenu()
        {
            tblSubMenu = new HashSet<tblSubMenu>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string MainMenu { get; set; }

        public int Position { get; set; }

        public virtual ICollection<tblSubMenu> tblSubMenu { get; set; }
    }
}
