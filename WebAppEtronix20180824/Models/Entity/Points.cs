namespace WebAppEtronix20180824.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public partial class Points
    {

        public int Id { get; set; }
        public string PointName { get; set; }
        public string TableName { get; set; }
        public string DatabaseName { get; set;}
        public string Tag1 { get; set; }
        public string Tag2 { get; set; }
        public string Tag3 { get; set; }
        public int MUT_id { get; set; }
        
    }


}