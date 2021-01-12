using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebAppEtronix20180824.App_Start.Other;

namespace WebAppEtronix20180824.Models.DTO
{
    public class PointsMU
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
            [Display(Name = "MU Id")]
            public int MUT_id { get; set; }
            [Display(Name = "Mu Name")]
            public string Type { get; set; }
            public EtronixValidation EtronixValidation { get; set; }

        
    }
}