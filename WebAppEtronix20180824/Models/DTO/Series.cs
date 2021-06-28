using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppEtronix20180824.Models.DTO
{
    public class SeriesData
    {
        public DateTime Timestamp { get; set; }
        public double ? Value { get; set; }

    
    }

    public class Root
    {
        public int Id { get; set; }
        public List<double ?> seriesdata { get; set; }
        public string seriesType { get; set; }
        public string seriesName { get; set; }
        public string seriesDashStyle { get; set; }
        public string seriesColor { get; set; }
        public int LineWidth { get; set; }
        public long pointStart { get; set; }
        public long pointInterval { get; set; }
        public string seriesYaxisTitle { get; set; }
        public string seriesYaxisColor { get; set; }
        public string seriesLabelFormat { get; set; }
        public string seriesLabelStyleColor { get; set; }
        public bool errorFlag { get; set; }
        public string errorMessage { get; set; }
    }

    public class PointDetails
    {
        public string PointName { get; set; }
        public string TableName { get; set; }
        public string DatabaseName { get; set; }

    }
}