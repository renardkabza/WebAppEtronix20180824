using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebAppEtronix20180824.App_Start.Other;

namespace WebAppEtronix20180824.Models
{
    public class PointViewModel
    {
        //public int Id { get; set; }

        [Required]
        [Display(Name = "Point Name")]
        public string PointName { get; set; }

        [Required]
        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        [Required]
        [Display(Name = "Database Name")]
        public string DataBaseName { get; set; }

        [Required]
        [Display(Name = "Measurement Unit Type")]
        public string MUT { get; set; }

        [Display(Name = "Tag 1")]
        public string  Tag1 { get; set; }

        [Display(Name = "Tag 2")]
        public string  Tag2 { get; set; }

        [Display(Name = "Tag 3")]
        public string  Tag3 { get; set; }

        

        public EtronixValidation EtronixValidation { get; set; }
    }
}