namespace WebAppEtronix20180824.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public partial class AccessPoints
    {

        public int Id { get; set; }
        public string ASPNetUserId { get; set; }
        public int PointId { get; set; }
        public bool Access { get; set; }

        
        
    }


}