using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WebAppEtronix20180824.App_Start.Other;

namespace WebAppEtronix20180824.Models
{
    
    
    public class MeasurementTypeViewModel
    {

        public string Id { get; set; }

        [Required]
        [Display(Name = "Measurement Type")]
        public string Type { get; set; }

        public EtronixValidation EtronixValidation { get; set; }

    }

    public class UpdateMeasurementTypeViewModel
    {

        [Required]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Measurement Type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "New Measurement Type")]
        public string NewType { get; set; }
        
        public EtronixValidation EtronixValidation { get; set; }
    }
}