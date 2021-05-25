using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppEtronix20180824.Models.DTO
{
    public class ChartModel
    {
        public ChartModel()
        {
            series = new List<Series>();

        }

        public List<Series> series { get; set; }
    }

    public class Series
    {
        public string name { get; set; }
        public string type { get; set; }
        public int yAxis { get; set; }
        public string color { get; set; }
        public List<double> data { get; set; }
        public Tooltip tooltip { get; set; }
        public int zIndex { get; set; }
    }

    public class Tooltip
    {
        public string valueSuffix { get; set; }
    }

 


}