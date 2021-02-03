using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    }
}