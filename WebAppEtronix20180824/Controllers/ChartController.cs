using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebAppEtronix20180824.Models.DTO;
using WebAppEtronix20180824.Models.Entity;

namespace WebAppEtronix20180824.Controllers
{
    public class ChartController : Controller
    {
        // GET: Chart
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult IndexTableActionResult(
          string pointName,
          string tableName,
          string databaseName,
          string tag1,
          string tag2,
          string tag3,
          int? mutID,
          string sortColumn,
          string sortOrder,
          int? currentpage,
          int? ppageSize)
        {
            var vPoints = PointsList(
                pointName,
                tableName,
                databaseName,
                tag1,
                tag2,
                tag3,
                mutID,
                sortColumn,
                sortOrder,
                currentpage,
                ppageSize);

            Debug.WriteLine(pointName);
            Debug.WriteLine(tableName);
            Debug.WriteLine(databaseName);
            Debug.WriteLine(tag1);
            Debug.WriteLine(tag2);
            Debug.WriteLine(tag3);
            Debug.WriteLine(mutID);
            Debug.WriteLine(sortColumn);
            Debug.WriteLine(sortOrder);
            Debug.WriteLine(currentpage);
            Debug.WriteLine(ppageSize);

            return PartialView("_Chart_IndexTable", vPoints);
        }

        private IPagedList<PointsMU> PointsList(
            string pointName,
            string tableName,
            string databaseName,
            string tag1,
            string tag2,
            string tag3,
            int? mutID,
            string sortColumn,
            string sortOrder,
            int? currentpage,
            int? ppageSize)
        {
            Entities db = new Entities();
            int pageIndex = Convert.ToInt32(currentpage);
            int? vsalary;

            bool pagehaschange = false;

            int pageSize = Convert.ToInt32(ppageSize);
            if (pageSize == 0)
            {
                pageSize = 1;
            }
            if (currentpage.Equals(null))
            {
                //pageIndex = currentpage.HasValue ? Convert.ToInt32(page) : 1;
                pageIndex = 1;
                ViewBag.CurrentPage = 1;
                ViewBag.PreviousPage = 1;
                pagehaschange = false;
            }

            if (currentpage <= 0)
            {
                pageIndex = 1;
                ViewBag.CurrentPage = 1;
                ViewBag.PreviousPage = 1;
                pagehaschange = false;
            }


            //ViewBag.CurrentSort = currentSort;
            //ViewBag.PreviousSort = previousSort;


            string vsortColumn = String.IsNullOrEmpty(sortColumn) ? "Point Name" : sortColumn;
            IPagedList<Points> vPoints = null;
            List<Points> vPointsMasters = null;
            ViewBag.SortColumn = vsortColumn;

            IPagedList<PointsMU> vPointsMU = null;

            ViewBag.CurrentPage = pageIndex;

            var flag = 1;
        takedata:
            switch (vsortColumn)
            {
                case "Point Name":
                    if (sortOrder == "Desc")
                    {
                        //employees = db.Employees.Where(m=>m.Name.Contains("10")).OrderByDescending(m => m.Name).ToPagedList(pageIndex, pageSize);
                        //or
                        //employees = db.Employees.OrderByDescending(m => m.Name).ToPagedList(pageIndex, pageSize);
                        //or some exmple from the internet
                        /*_db.EMPLOYEEs.Where(x => x.EMAIL == givenInfo || x.USER_NAME == givenInfo)
                        .Select(x => new { x.EMAIL, x.ID }); */
                        vPointsMasters = db.Points.OrderByDescending(m => m.PointName).ToList();

                    }
                    else
                    {
                        vPointsMasters = db.Points.OrderBy(m => m.PointName).ToList();
                    }

                    break;
                case "Table Name":
                    if (sortOrder == "Desc")
                        vPointsMasters = db.Points.OrderByDescending(m => m.TableName).ToList();
                    else
                        vPointsMasters = db.Points.OrderBy(m => m.TableName).ToList();
                    break;
                case "Database Name":
                    if (sortOrder == "Desc")
                        vPointsMasters = db.Points.OrderByDescending(m => m.DatabaseName).ToList();
                    else
                        vPointsMasters = db.Points.OrderBy(m => m.DatabaseName).ToList();
                    break;
                case "Tag 1":
                    if (sortOrder == "Desc")
                        vPointsMasters = db.Points.OrderByDescending(m => m.Tag1).ToList();
                    else
                        vPointsMasters = db.Points.OrderBy(m => m.Tag1).ToList();
                    break;
                case "Tag 2":
                    if (sortOrder == "Desc")
                        vPointsMasters = db.Points.OrderByDescending(m => m.Tag2).ToList();
                    else
                        vPointsMasters = db.Points.OrderBy(m => m.Tag2).ToList();
                    break;
                case "Tag 3":
                    if (sortOrder == "Desc")
                        vPointsMasters = db.Points.OrderByDescending(m => m.Tag3).ToList();
                    else
                        vPointsMasters = db.Points.OrderBy(m => m.Tag3).ToList();
                    break;
                case "MUT":
                    if (sortOrder == "Desc")
                        vPointsMasters = db.Points.OrderByDescending(m => m.MUT_id).ToList();
                    else
                        vPointsMasters = db.Points.OrderBy(m => m.MUT_id).ToList();
                    break;


                default: //by Employee Name and ASC
                    vPoints = db.Points.OrderBy(m => m.PointName).ToPagedList(pageIndex, pageSize);
                    break;
            }

            if (pointName != "")
            {
                vPointsMasters = vPointsMasters.Where(m => m.PointName.Contains(pointName)).ToList();
            }
            if (tableName != "")
            {
                vPointsMasters = vPointsMasters.Where(m => m.TableName.Contains(tableName)).ToList();
            }
            if (databaseName != "")
            {
                vPointsMasters = vPointsMasters.Where(m => m.DatabaseName.Contains(databaseName)).ToList();
            }
            if (tag1 != "")
            {
                vPointsMasters = vPointsMasters.Where(m => m.Tag1.Contains(tag1)).ToList();
            }
            if (tag2 != "")
            {
                vPointsMasters = vPointsMasters.Where(m => m.Tag2.Contains(tag2)).ToList();
            }
            if (tag3 != "")
            {
                vPointsMasters = vPointsMasters.Where(m => m.Tag3.Contains(tag3)).ToList();
            }
            if (mutID.HasValue)
            {
                vPointsMasters = vPointsMasters.Where(m => m.MUT_id.ToString().Contains(mutID.ToString())).ToList();
            }


            //vPoints = vPointsMasters.ToPagedList(pageIndex, pageSize);

            List<Procedures.MeasurementUnitTypes> vM = db.MeasurementUnitType.ToList();

            List<PointsMU> vCombinedLists = vPointsMasters.Join(vM, a => a.MUT_id, b => b.Id, (a, b) => new PointsMU
            {
                Id = a.Id,
                PointName = a.PointName,
                TableName = a.TableName,
                DatabaseName = a.DatabaseName,
                Tag1 = a.Tag1,
                Tag2 = a.Tag2,
                Tag3 = a.Tag3,
                MUT_id = a.MUT_id,
                Type = b.Type
            }).ToList();

            vPointsMU = vCombinedLists.ToPagedList(pageIndex, pageSize);

            int rowsquantity = vPointsMU.Count;

            //rowsquantity = vCombinedLists.Count;


            if (flag == 1)
            {
                flag = 0;
                if (rowsquantity == 0)
                {
                    currentpage = 1;
                    ViewBag.CurrentPage = 1;
                    pageIndex = 1;

                    goto takedata;
                }
            }
            return vPointsMU;
        }

        //Goto Previous Page
        public ActionResult PreviousPageActionResult(
            string pointName,
            string tableName,
            string databaseName,
            string tag1,
            string tag2,
            string tag3,
            int? mutID,
            string sortColumn,
            string sortOrder,
            int? currentpage,
            int? ppageSize)
        {

            int? v_currentpage = currentpage;
            v_currentpage--;

            var vPoints = PointsList(
                pointName,
                tableName,
                databaseName,
                tag1,
                tag2,
                tag3,
                mutID,
                sortColumn,
                sortOrder,
                v_currentpage,
                ppageSize);


            return PartialView("_Chart_IndexTable", vPoints);



        }

        //Goto Previous Page
        public ActionResult NextPageActionResult(
            string pointName,
            string tableName,
            string databaseName,
            string tag1,
            string tag2,
            string tag3,
            int? mutID,
            string sortColumn,
            string sortOrder,
            int? currentpage,
            int? ppageSize)
        {

            int? v_currentpage = currentpage;
            v_currentpage++;
            var vPoints = PointsList(
                pointName,
                tableName,
                databaseName,
                tag1,
                tag2,
                tag3,
                mutID,
                sortColumn,
                sortOrder,
                v_currentpage,
                ppageSize);


            return PartialView("_Chart_IndexTable", vPoints);



        }

        public ActionResult FetchChartSerie(
            int PointId,
            string ChartType,
            string DateRange,
            string AggregationInterval,
            string AggregationType,
            string DateFrom,
            string DateTo,
            string SeriesColor,
            string SeriesDashStyle,
            string MU
            )

        {
           



            //Get hex color

            Entities _contextEntities = new Entities();
            var vColorName = new SqlParameter("@ColorName", SeriesColor.Substring(3));

            string vHexColor = _contextEntities.Database.SqlQuery<System.String>("GetColor " +"@ColorName", vColorName).FirstOrDefault();



            //GetTables Details
            
            var vPointDetailSqlParameter = new SqlParameter("@PointId", PointId);

            PointDetails vPointDetails = _contextEntities.Database
                .SqlQuery<PointDetails>("GetPointDetails " + "@PointId", vPointDetailSqlParameter).FirstOrDefault();




            //Prepare 
            List<SeriesData> vSeriesData = new List<SeriesData>();

            var vTableName = new SqlParameter("@TableName", vPointDetails.TableName );
            var vDBName = new SqlParameter("@DataBaseName", vPointDetails.DatabaseName );
            var vDateTimeFrom = new SqlParameter("@DateTimeStart", DateFrom );
            var vDateTimeTo = new SqlParameter("@DateTimeStop", DateTo );

            int[] vIntervalTab = PrepareIntervals(AggregationInterval);

            int step = 0;
            if (ReccommendedSetp(Convert.ToDateTime(DateFrom), Convert.ToDateTime(DateTo)) > vIntervalTab[0])
            {
                step = ReccommendedSetp(Convert.ToDateTime(DateFrom), Convert.ToDateTime(DateTo));
            }
            else
            {
                step = vIntervalTab[0];
            }
            //var vAggregationInterval = new SqlParameter("@AggregationInterval", vIntervalTab[0].ToString());
            var vAggregationInterval = new SqlParameter("@AggregationInterval", step.ToString());
            var vAggregationFunction = new SqlParameter("@AggregationFunction", AggregationType);
            var vAggregationNDays = new SqlParameter("@AggregationNDays", vIntervalTab[1].ToString());
            var vAggregationFunctionCum = new SqlParameter("@AggregationFunctionCum", "Null");


            

            vSeriesData = _contextEntities.Database.SqlQuery<SeriesData>("GetSeriesAggregationDynamicNMinutesNDays_TestV2 " +
                                                           "@TableName, " +
                                                           "@DataBaseName, " +
                                                           "@DateTimeStart, " +
                                                           "@DateTimeStop, " +
                                                           "@AggregationInterval, " +
                                                           "@AggregationFunction, " +
                                                           "@AggregationNDays, " +
                                                           "@AggregationFunctionCum " ,
                                                           vTableName,
                                                           vDBName,
                                                           vDateTimeFrom,
                                                           vDateTimeTo,
                                                           vAggregationInterval,
                                                           vAggregationFunction,
                                                           vAggregationNDays,
                                                           vAggregationFunctionCum).ToList();
             




            //ulong Max = 60;//8760 * 1 + 1;
            //float vd = 0;

            Root vObject = new Root();
            vObject.seriesdata = new List<double ?>();

            Random vRandom = new Random();
            
            for (int i = 0; i < vSeriesData.Count; i++)
            {
                //time = (ulong)(1609459200000 + ((i * 60) * 1000));
                //vd = vRandom.Next(-100, 100);

                vObject.seriesdata.Add(vSeriesData[i].Value); //vSeriesData[i].Value
            }
            
            /*
            string a = "[";
            for (int i = 0; i < vSeriesData.Count; i++)
            {
                a += "[";
                var time = GetCurrentUnixTimestampMillis(vSeriesData[i].Timestamp);
                a += time;
                a += ",";
                var vd = vSeriesData[i].Value;
                a += vd;
                a += "],";
                //vObject.seriesdata.Add(vSeriesData[i].Value);
            }
            a = a.Remove(a.LastIndexOf(','), 1);
            a += "]";
            vObject.seriesdata = a;
             */
            //Get the start Point

            long vDateTimeStart = GetCurrentUnixTimestampMillis(Convert.ToDateTime(DateFrom));
            long vInterval0 = LongGetIntervalInMilliseconds(AggregationInterval);
            long vInterval1 = ReccommendedSetpInmiliseconds(Convert.ToDateTime(DateFrom), Convert.ToDateTime(DateTo));

            long vInterval = 0;

            if (vInterval0 > vInterval1)
            {
                vInterval = vInterval0;
            }
            else
            {
                vInterval = vInterval1;
            }

            vObject.Id = PointId;
            vObject.seriesType = "spline";
            vObject.seriesName = vPointDetails.PointName;
            vObject.seriesDashStyle = SeriesDashStyle;
            vObject.seriesColor = vHexColor;
            vObject.LineWidth = 2;
            vObject.pointStart = vDateTimeStart; //1609459200000; unix ms from the epoch
            vObject.pointInterval = vInterval;
            vObject.seriesYaxisTitle = "";
            vObject.seriesYaxisColor = vHexColor;
            vObject.seriesLabelFormat = MU;
            vObject.seriesLabelStyleColor = vHexColor;
            vObject.errorFlag = false;
            vObject.errorMessage = "";



            //return Json(vObject, JsonRequestBehavior.AllowGet);

            var jsonResult = Json(vObject, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;



            //return null;
        }

        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis(DateTime pDateTime)
        {
            return (long)(pDateTime - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        public static long LongGetIntervalInMilliseconds(string pInterval)
        {
            long vInterval = 60000;
            switch (pInterval)
            {
                case "1m":
                    vInterval = vInterval*1;
                    break;
                case "5m":
                    vInterval = vInterval*5;
                    break;
                case "10m":
                    vInterval = vInterval*10;
                    break;
                case "15m":
                    vInterval = vInterval * 15;
                    break;
                case "20m":
                    vInterval = vInterval * 30;
                    break;
                case "30m":
                    vInterval = vInterval * 30;
                    break;
                case "1h":
                    vInterval = vInterval * 60;
                    break;
                case "2h":
                    vInterval = vInterval * 60*2;
                    break;
                case "3h":
                    vInterval = vInterval * 60 * 3;
                    break;
                case "4h":
                    vInterval = vInterval * 60 * 4;
                    break;
                case "6h":
                    vInterval = vInterval * 60 * 6;
                    break;
                case "12h":
                    vInterval = vInterval * 60 * 12;
                    break;
                case "1D":
                    vInterval = vInterval * 60 * 24;
                    break;
                case "2D":
                    vInterval = vInterval * 60 * 24*2;
                    break;
                case "3D":
                    vInterval = vInterval * 60 * 24 * 3;
                    break;
                case "4D":
                    vInterval = vInterval * 60 * 24 * 4;
                    break;
                case "5D":
                    vInterval = vInterval * 60 * 24 * 5;
                    break;
                case "6D":
                    vInterval = vInterval * 60 * 24 * 6;
                    break;
                case "1W":
                    vInterval = vInterval * 60 * 24 * 7;
                    break;
                case "2W":
                    vInterval = vInterval * 60 * 24 * 7*2;
                    break;
                case "3W":
                    vInterval = vInterval * 60 * 24 * 7 * 3;
                    break;
                case "1M":
                    vInterval = vInterval * 60 * 24  * 31;
                    break;
                case "2M":
                    vInterval = vInterval * 60 * 24 * 31*2;
                    break;
                case "3M":
                    vInterval = vInterval * 60 * 24  * 31 * 3;
                    break;
                case "4M":
                    vInterval = vInterval * 60 * 24  * 31 * 4;
                    break;
                case "6M":
                    vInterval = vInterval * 60 * 24  * 31 * 6;
                    break;
                case "1Y":
                    vInterval = vInterval * 60 * 24  * 31 * 12;
                    break;

                default:
                    //1m
                    vInterval = 60000;
                    break;

                    
            }

            return vInterval;
        }

        public static int [] PrepareIntervals (string pInterval)
        {
            int vInterval = 1;
            int vNDays =0;

            int[] vVariableInt = new int[2];


            switch (pInterval)
            {
                case "1m":
                    vInterval = 1;
                    break;
                case "5m":
                    vInterval = 5;
                    break;
                case "10m":
                    vInterval = 10;
                    break;
                case "15m":
                    vInterval = 15;
                    break;
                case "20m":
                    vInterval = 20;
                    break;
                case "30m":
                    vInterval = 30;
                    break;
                case "1h":
                    vInterval = 60;
                    break;
                case "2h":
                    vInterval = 60*2;
                    break;
                case "3h":
                    vInterval = 60 * 3;
                    break;
                case "4h":
                    vInterval = 60 * 4;
                    break;
                case "6h":
                    vInterval = 360;
                    break;
                case "12h":
                    vInterval = 720;
                    break;
                case "1D":
                    vInterval = 1440;
                    break;
                case "2D":
                    vInterval = 1440;
                    vNDays = 2;
                    break;
                case "3D":
                    vInterval = 1440;
                    vNDays = 3;
                    break;
                case "4D":
                    vInterval = 1440;
                    vNDays = 4;
                    break;
                case "5D":
                    vInterval = 1440;
                    vNDays = 5;
                    break;
                case "6D":
                    vInterval = 1440;
                    vNDays = 6;
                    break;
                case "1W":
                    vInterval = 1440;
                    vNDays = 7;
                    break;
                case "2W":
                    vInterval = 1440;
                    vNDays = 14;
                    break;
                case "3W":
                    vInterval = 1440;
                    vNDays = 21;
                    break;
                case "1M":
                    vInterval = vInterval * 60 * 24  * 31;
                    break;
                case "2M":
                    vInterval = vInterval * 60 * 24 * 31*2;
                    break;
                case "3M":
                    vInterval = vInterval * 60 * 24  * 31 * 3;
                    break;
                case "4M":
                    vInterval = vInterval * 60 * 24  * 31 * 4;
                    break;
                case "6M":
                    vInterval = vInterval * 60 * 24  * 31 * 6;
                    break;
                case "1Y":
                    vInterval = vInterval * 60 * 24  * 31 * 12;
                    break;

                default:
                    //1m
                    vInterval = 60000;
                    break;

                    
            }

            vVariableInt[0] = vInterval;
            vVariableInt[1] = vNDays;

            return vVariableInt;

        }

        public ActionResult FetchChartSerieZoomed(
            int PointId,
            string ChartType,
            string DateRange,
            string AggregationInterval,
            string AggregationType,
            string DateFrom,
            string DateTo,
            string SeriesColor,
            string SeriesDashStyle,
            string MU
            )
        {




            //Get hex color

            Entities _contextEntities = new Entities();
            var vColorName = new SqlParameter("@ColorName", SeriesColor.Substring(3));

            string vHexColor = _contextEntities.Database.SqlQuery<System.String>("GetColor " + "@ColorName", vColorName).FirstOrDefault();



            //GetTables Details

            var vPointDetailSqlParameter = new SqlParameter("@PointId", PointId);

            PointDetails vPointDetails = _contextEntities.Database
                .SqlQuery<PointDetails>("GetPointDetails " + "@PointId", vPointDetailSqlParameter).FirstOrDefault();




            //Prepare 
            List<SeriesData> vSeriesData = new List<SeriesData>();

            var vTableName = new SqlParameter("@TableName", vPointDetails.TableName);
            var vDBName = new SqlParameter("@DataBaseName", vPointDetails.DatabaseName);
            DateTime vDTStart = UnixTimeStampToDateTime(double.Parse(DateFrom));

            var vDateTimeFrom = new SqlParameter("@DateTimeStart", vDTStart);
            DateTime vDTStop = UnixTimeStampToDateTime(double.Parse(DateTo));
            var vDateTimeTo = new SqlParameter("@DateTimeStop", vDTStop);

            int[] vIntervalTab = PrepareIntervals(AggregationInterval);

            /*
            int step = 0;
            if (ReccommendedSetp(Convert.ToDateTime(DateFrom), Convert.ToDateTime(DateTo)) > vIntervalTab[0])
            {
                step = ReccommendedSetp(Convert.ToDateTime(DateFrom), Convert.ToDateTime(DateTo));
            }
            else
            {
                step = vIntervalTab[0];
            }
            */
            var vAggregationInterval = new SqlParameter("@AggregationInterval", ReccommendedSetp(Convert.ToDateTime(vDTStart), Convert.ToDateTime(vDTStop))); //vIntervalTab[0].ToString());
            var vAggregationFunction = new SqlParameter("@AggregationFunction", AggregationType);
            var vAggregationNDays = new SqlParameter("@AggregationNDays", vIntervalTab[1].ToString());
            var vAggregationFunctionCum = new SqlParameter("@AggregationFunctionCum", "Null");




            vSeriesData = _contextEntities.Database.SqlQuery<SeriesData>("GetSeriesAggregationDynamicNMinutesNDays_TestV2 " +
                                                           "@TableName, " +
                                                           "@DataBaseName, " +
                                                           "@DateTimeStart, " +
                                                           "@DateTimeStop, " +
                                                           "@AggregationInterval, " +
                                                           "@AggregationFunction, " +
                                                           "@AggregationNDays, " +
                                                           "@AggregationFunctionCum ",
                                                           vTableName,
                                                           vDBName,
                                                           vDateTimeFrom,
                                                           vDateTimeTo,
                                                           vAggregationInterval,
                                                           vAggregationFunction,
                                                           vAggregationNDays,
                                                           vAggregationFunctionCum).ToList();





            //ulong Max = 60;//8760 * 1 + 1;
            //float vd = 0;

            Root vObject = new Root();
            vObject.seriesdata = new List<double?>();

            Random vRandom = new Random();
            for (int i = 0; i < vSeriesData.Count; i++)
            {
                vObject.seriesdata.Add(vSeriesData[i].Value);
            }
            
            /*
            string a = "[";
            for (int i = 0; i < vSeriesData.Count; i++)
            {
                a += "[";
                var time = vSeriesData[i].Timestamp;
                a += time;
                a += ",";
                var vd = vSeriesData[i].Value;
                a += vd;
                a += "],";
                //vObject.seriesdata.Add(vSeriesData[i].Value);
            }
            a=a.Remove(a.LastIndexOf(','), 1);
            a += "]";
            vObject.seriesdata = a;
             * */
            //Get the start Point

            long vDateTimeStart = long.Parse(DateFrom); //GetCurrentUnixTimestampMillis(Convert.ToDateTime(DateFrom));
            long vInterval = LongGetIntervalInMilliseconds(AggregationInterval);

            //long vInterval0 = LongGetIntervalInMilliseconds(AggregationInterval);
            //long vInterval1 = ReccommendedSetpInmiliseconds(Convert.ToDateTime(DateFrom), Convert.ToDateTime(DateTo));

            /*
            long vInterval = 0;

            if (vInterval0 > vInterval1)
            {
                vInterval = vInterval0;
            }
            else
            {
                vInterval = vInterval1;
            }
             **/

            vObject.Id = PointId;
            vObject.seriesType = "spline";
            vObject.seriesName = vPointDetails.PointName;
            vObject.seriesDashStyle = SeriesDashStyle;
            vObject.seriesColor = vHexColor;
            vObject.LineWidth = 2;
            vObject.pointStart = vDateTimeStart; //1609459200000; unix ms from the epoch
            vObject.pointInterval = ReccommendedSetpInmiliseconds(Convert.ToDateTime(vDTStart), Convert.ToDateTime(vDTStop));//vInterval;
            vObject.seriesYaxisTitle = "";
            vObject.seriesYaxisColor = vHexColor;
            vObject.seriesLabelFormat = MU;
            vObject.seriesLabelStyleColor = vHexColor;
            vObject.errorFlag = false;
            vObject.errorMessage = "";



            //return Json(vObject, JsonRequestBehavior.AllowGet);

            var jsonResult = Json(vObject, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;



            //return null;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public int ReccommendedSetp(DateTime p1, DateTime p2)
        {
            TimeSpan span = p2.Subtract(p1);

            int howManyDays = span.Days;

            if (howManyDays > 180 )
            {
                return 1440;
            }
            else if (howManyDays > 90)
            {
                return 240;
            }
            else if (howManyDays >31)
            {
                return 240;
            }
            else if (howManyDays >1)
            {
                return 60;
            }
            else
            {
                return 1;
            }

        }

        public long ReccommendedSetpInmiliseconds(DateTime p1, DateTime p2)
        {
            TimeSpan span = p2.Subtract(p1);

            int howManyDays = span.Days;
            long minute = 60000;
            if (howManyDays > 180)
            {
                return 1440 * minute;
            }
            else if (howManyDays > 90)
            {
                return 240 * minute;
            }
            else if (howManyDays > 31)
            {
                return 240 * minute;
            }
            else if (howManyDays > 1)
            {
                return 60 * minute;
            }
            else
            {
                return 1 * minute;
            }

        }
    }
}