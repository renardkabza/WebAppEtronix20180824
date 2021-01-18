using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebAppEtronix20180824.App_Start.Other;

namespace WebAppEtronix20180824.Models.DTO
{
    public class AccessPo
    {

            [Display(Name = "Point Id")]
            public int Id { get; set; }
            [Display(Name = "Point Name")]
            public string PointName { get; set; }
            [Display(Name = "Table Name")]
            public string TableName { get; set; }
            [Display(Name = "Database Name")]
            public string DatabaseName { get; set; }
            [Display(Name = "Tag 1")]
            public string Tag1 { get; set; }
            [Display(Name = "Tag 2")]
            public string Tag2 { get; set; }
            [Display(Name = "Tag 3")]
            public string Tag3 { get; set; }
        
        
        
            //[Display(Name = "Access Id")]
            //public int Id { get; set; }
            [Display(Name = "User Id")]
            public string ASPNetUserId { get; set; }
            [Display(Name = "Point Id")]    
            public int PointId { get; set; }
            [Display(Name = "Access")]
            public bool Access { get; set; }
            
            
            
            public EtronixValidation EtronixValidation { get; set; }

        
    }

    public class PointUpdate
    {
        
        public string PtId { get; set; }
        public bool PtValue { get; set; }
        
    }


    public class PointUpdateAll
    {
        public List<PointUpdate> pointsList { get; set; }
    }

}