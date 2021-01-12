using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppEtronix20180824.App_Start.Other
{
    
    public class EtronixValidation
    {


        public List<string> ValidationMessage;
        
        public string GetFromList (int pindex)
        {
            return ValidationMessage[pindex].ToString();

        }
        public void AddToList(string pvalue)
        {
             ValidationMessage.Add(pvalue); 
        }

        private string validationCode;

        public string ValidationCode
        {
            get { return this.validationCode; }
            set { this.validationCode = value; }

        }

        public EtronixValidation()
        {
            ValidationMessage = new List<string>();
            
        }
    }

    public static class EtronixValidationCode
    {
        public enum ValidationCodeEnum
        {
            alert_success = 0,
            alert_primary = 1,
            alert_info = 2,
            alert_secondary = 3,
            alert_light = 4,
            alert_warning = 5,
            alert_danger = 6
        }

        public static IDictionary<int, string> ValidationDic = new Dictionary<int, string>()
        {
            {0,"alert-success"},
            {1,"alert-primary"},
            {2,"alert-info"},
            {3,"alert-secondary"},
            {4,"alert-light"},
            {5,"alert-warning"},
            {6,"alert-danger"}
            
        };

        public static IDictionary<int,string> ValidationDicRev = new Dictionary<int, string>()
        {
            {0, "Success"},
            {1, "Info No.1"},
            {2, "Info No.2"},
            {3, "Alert No.1"},
            {4, "AlerNo.2"},
            {5, "Warning"},
            {6, "Danger"},
            
        };
    }
}