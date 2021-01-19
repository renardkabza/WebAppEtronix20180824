using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PagedList;
using WebAppEtronix20180824.App_Start.Other;
using WebAppEtronix20180824.Models.DTO;
using WebAppEtronix20180824.Models.Entity;

namespace WebAppEtronix20180824.Controllers
{
    public class AccessController : Controller
    {
        //Firsl Load
        public ActionResult Index(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {
            return View();
        }

        //Get: Return a tablet as a partial view
        public ActionResult IndexTableActionResult(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {
            var vUsers = AspNetUsers(userName, email, sortColumn, sortOrder, currentpage, ppageSize);


            return PartialView("_Access_IndexTable", vUsers);
        }

        //Users
        private IPagedList<AspNetUsers> AspNetUsers(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
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


            string vsortColumn = String.IsNullOrEmpty(sortColumn) ? "User Name" : sortColumn;
            IPagedList<AspNetUsers> vAspNetUsers = null;
            List<AspNetUsers> vAspNetUsersMasters = null;
            ViewBag.SortColumn = vsortColumn;


            ViewBag.CurrentPage = pageIndex;

            var flag = 1;
        takedata:
            switch (vsortColumn)
            {
                case "User Name":
                    if (sortOrder == "Desc")
                    {
                        //employees = db.Employees.Where(m=>m.Name.Contains("10")).OrderByDescending(m => m.Name).ToPagedList(pageIndex, pageSize);
                        //or
                        //employees = db.Employees.OrderByDescending(m => m.Name).ToPagedList(pageIndex, pageSize);
                        //or some exmple from the internet
                        /*_db.EMPLOYEEs.Where(x => x.EMAIL == givenInfo || x.USER_NAME == givenInfo)
                        .Select(x => new { x.EMAIL, x.ID }); */
                        vAspNetUsersMasters = db.AspNetUsers.OrderByDescending(m => m.UserName).ToList();

                    }
                    else
                    {
                        vAspNetUsersMasters = db.AspNetUsers.OrderBy(m => m.UserName).ToList();
                    }

                    break;
                case "Email":
                    if (sortOrder == "Desc")
                        vAspNetUsersMasters = db.AspNetUsers.OrderByDescending(m => m.Email).ToList();
                    else
                        vAspNetUsersMasters = db.AspNetUsers.OrderBy(m => m.Email).ToList();
                    break;
                default: //by Employee Name and ASC
                    vAspNetUsers = db.AspNetUsers.OrderBy(m => m.UserName).ToPagedList(pageIndex, pageSize);
                    break;
            }

            if (userName != "")
            {
                vAspNetUsersMasters = vAspNetUsersMasters.Where(m => m.UserName.Contains(userName)).ToList();
            }
            if (email != "")
            {
                vAspNetUsersMasters = vAspNetUsersMasters.Where(m => m.Email.Contains(email)).ToList();
            }

            vAspNetUsers = vAspNetUsersMasters.ToPagedList(pageIndex, pageSize);


            int rowsquantity = vAspNetUsers.Count;


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
            return vAspNetUsers;
        }

        //Goto Previous Page
        public ActionResult PreviousPageActionResult(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {

            int? v_currentpage = currentpage;
            v_currentpage--;
            var employees = AspNetUsers(userName, email, sortColumn, sortOrder, v_currentpage, ppageSize);


            return PartialView("_Access_IndexTable", employees);



        }

        //Goto Previous Page
        public ActionResult NextPageActionResult(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {

            int? v_currentpage = currentpage;
            v_currentpage++;
            var employees = AspNetUsers(userName, email, sortColumn, sortOrder, v_currentpage, ppageSize);


            return PartialView("_Access_IndexTable", employees);



        }

        //*Access



        public ActionResult IndexTableActionResult_2(
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
           int? ppageSize,
            string userId)
        {
            var vPoints = PointsList_2(
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
                ppageSize,
                userId);

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

            return PartialView("_Access_IndexTable_2", vPoints);
        }

        private IPagedList<AccessPo> PointsList_2(
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
            int? ppageSize,
            string puserId)
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
            IPagedList<AccessPo> vAccessPoints = null;
            List<Points> vPointsMasters = null;
            ViewBag.SortColumn = vsortColumn;

            //IPagedList<PointsMU> vPointsMU = null;

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
                    //vPoints = db.Points.OrderBy(m => m.PointName).ToPagedList(pageIndex, pageSize);
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

            List<AccessPoints> vM = db.AccessP.ToList();

            vM = vM.Where(m => m.ASPNetUserId.Contains(puserId)).ToList();

            List<AccessPo> vCombinedLists = vPointsMasters.Join(vM, a => a.Id, b => b.PointId, (a, b) => new AccessPo
            {
                PointId = b.PointId,
                PointName = a.PointName,
                TableName = a.TableName,
                DatabaseName = a.DatabaseName,
                Tag1 = a.Tag1,
                Tag2 = a.Tag2,
                Tag3 = a.Tag3,
                Id=b.Id,
                ASPNetUserId =b.ASPNetUserId,
                Access = b.Access
                
                
            }).ToList();

            vAccessPoints = vCombinedLists.ToPagedList(pageIndex, pageSize);
            
            int rowsquantity = vAccessPoints.Count;
            

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
            return vAccessPoints;
        }

        //Goto Previous Page
        public ActionResult NextPageActionResult2(
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
            int? ppageSize,
            string userId)
        {

            int? v_currentpage = currentpage;
            v_currentpage++;
            var vPoints = PointsList_2(
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
                ppageSize,
                userId);


            return PartialView("_Access_IndexTable_2", vPoints);



        }

        //Goto Previous Page
        public ActionResult PreviousPageActionResult2(
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
            int? ppageSize,
            string userId)
        {

            int? v_currentpage = currentpage;
            v_currentpage--;
            var vPoints = PointsList_2(
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
                ppageSize,
                userId);


            return PartialView("_Access_IndexTable_2", vPoints);



        }

        public ActionResult UpdatePoints
        (
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
            int? ppageSize,
            string userId,
            string pointsList)
        {

            
            // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
            //https://json2csharp.com/json-to-csharp
            PointUpdateAll vAllPoints = JsonConvert.DeserializeObject<PointUpdateAll>(pointsList);


            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;

            Entities _contextEntities = new Entities();
            try
            {


                //Update All points for the choses user from the particular page
                foreach (PointUpdate element in vAllPoints.pointsList)
                {
                    var vUserId = new SqlParameter("@UserId", userId);
                    var vId = new SqlParameter("@Id", element.PtId.Substring(2));
                    var vAccess = new SqlParameter("@Access", element.PtValue);

                    var vRow = _contextEntities.Database.SqlQuery<Procedures.ResultAccess>(
                        "UpdateAccessPoint @UserId, @Id , @Access ", 
                        vUserId,
                        vId,
                        vAccess
                    ).ToList();

                }
            }
            catch (Exception exception)
            {
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vMessage = exception.Message;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);
                //model.EtronixValidation = EV;
            }

            int? v_currentpage = currentpage;
            //stay on teh same page and update the points list
            //v_currentpage--;

            var vPoints = PointsList_2(
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
                ppageSize,
                userId);

            //return just a confirmation
            return PartialView("_Access_IndexTable_2", vPoints);
        }
    }
}