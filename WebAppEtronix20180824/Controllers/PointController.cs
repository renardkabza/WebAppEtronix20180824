using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PagedList;
using WebAppEtronix20180824.App_Start.Other;
using WebAppEtronix20180824.Models;
using WebAppEtronix20180824.Models.DTO;
using WebAppEtronix20180824.Models.Entity;

namespace WebAppEtronix20180824.Controllers
{
    public class PointController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        
        
        
        
        // GET: Point
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetData()
        {
            List<MeasurementUnit> vList =  new List<MeasurementUnit>();

            /*
            MeasurementUnit vMu = new MeasurementUnit();
            vMu.MUType="New";
            vMu.MUid = 1;

            MeasurementUnit vMu2 = new MeasurementUnit();
            vMu2.MUType = "New2";
            vMu2.MUid = 2;

            vList.Add(vMu);
            vList.Add(vMu2);
            */
            Entities _contextEntities = new Entities();
            //var vMeasurementType = new SqlParameter("@Type", pDataViewModel.Type);
            List<Procedures.MeasurementUnitTypes> vMeasurementUnitTypes =
                _contextEntities.Database.SqlQuery<Procedures.MeasurementUnitTypes>("GetAllMeasurementUnits").ToList();

            foreach (var item in vMeasurementUnitTypes)
            {
                MeasurementUnit vMeasurementUnit = new MeasurementUnit();
                vMeasurementUnit.MUType = item.Type;
                vMeasurementUnit.MUid = item.Id.ToString();
                vList.Add(vMeasurementUnit);
            }

            var json = JsonConvert.SerializeObject(vList);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(PointViewModel model)
        {
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;
            Entities _contextEntities = new Entities();

            try
            {

            if (ModelState.IsValid)
            {


                

                var vPointPN = new SqlParameter("@PointName", model.PointName);
                var vPointTN = new SqlParameter("@TableName", model.TableName);
                var vPointDBN = new SqlParameter("@DatabaseName", model.DataBaseName);

                //the optional fields
                if (model.Tag1 == null)
                {
                    model.Tag1 ="";
                }
                if (model.Tag2 == null)
                {
                    model.Tag2 = "";
                }
                if (model.Tag3 == null)
                {
                    model.Tag3 = "";
                }
                var vPointTag1 = new SqlParameter("@Tag1", model.Tag1);
                var vPointTag2 = new SqlParameter("@Tag2", model.Tag2);
                var vPointTag3 = new SqlParameter("@Tag3", model.Tag3);

                //get MUT ID by MUT Name
                var vPointMUT_0 = new SqlParameter("@PointMUT_Name", model.MUT);
                var vPointMutId_0 = _contextEntities.Database.SqlQuery<int>("GetIdMUT " + "@PointMUT_Name ", vPointMUT_0).SingleOrDefault();

                
                // Set MUT ID
                var vPointMUT_1 = new SqlParameter("@MUT_Id", vPointMutId_0);

                var vPointExists =
                    _contextEntities.Database.SqlQuery<String>("CheckPointIfExist " +
                    "@PointName,  @TableName, @DatabaseName, @Tag1, @Tag2, @Tag3, @MUT_id",
                    vPointPN, vPointTN, vPointDBN, vPointTag1, vPointTag2, vPointTag3, vPointMUT_1).SingleOrDefault();
                Debug.WriteLine("The points already exists:" +vPointExists);


                if (Boolean.Parse(vPointExists) == true)
                {
                    //Generate message
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    vMessage = "There is already a point in the database with the following parameters:";
                    EV.AddToList(vMessage);
                    //ID but I will not put it here
                    
                    vMessage = "Point Name:" + model.PointName + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Table Name:" + model.TableName + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Database Name:" + model.DataBaseName + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Tag 1:" + model.Tag1 + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Tag 2:" + model.Tag2 + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Tag 3:" + model.Tag3 + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "MUT:" + model.MUT + Environment.NewLine;
                    EV.AddToList(vMessage);

                    //update model
                    model.EtronixValidation = EV;
                    goto _end;
                }
                else
                {

                    //select Id of MUT measurement unit type
                    
                    var vPointMUT = new SqlParameter("@PointMUT_Name", model.MUT);
                    var vPointMutId = _contextEntities.Database.SqlQuery<int>("GetIdMUT " + "@PointMUT_Name ", vPointMUT).SingleOrDefault();
                    Debug.WriteLine("MUT ID:"+vPointMutId);

                    
                    //set SQL params
                    var vPointPN_2 = new SqlParameter("@PointName", model.PointName);
                    var vPointTN_2 = new SqlParameter("@TableName", model.TableName);
                    var vPointDBN_2 = new SqlParameter("@DatabaseName", model.DataBaseName);

                    //the optional fields
                    if (model.Tag1 == null)
                    {
                        model.Tag1 = "";
                    }
                    if (model.Tag2 == null)
                    {
                        model.Tag2 = "";
                    }
                    if (model.Tag3 == null)
                    {
                        model.Tag3 = "";
                    }
                    var vPointTag1_2 = new SqlParameter("@Tag1", model.Tag1);
                    var vPointTag2_2 = new SqlParameter("@Tag2", model.Tag2);
                    var vPointTag3_2 = new SqlParameter("@Tag3", model.Tag3);

                    
                    // Set MUT ID
                    var vPointMUT_2 = new SqlParameter("@MUT_Id", vPointMutId);
                    

                    

                    //insert new point
                    Procedures.ResultInsertPoint vPointInserted =
                        _contextEntities.Database.SqlQuery<Procedures.ResultInsertPoint>
                        ("InsertPoints " + "@PointName,  @TableName, @DatabaseName, @Tag1, @Tag2, @Tag3, @MUT_id",
                            vPointPN_2, vPointTN_2, vPointDBN_2, vPointTag1_2, vPointTag2_2, vPointTag3_2, vPointMUT_2).SingleOrDefault();
                    Debug.WriteLine("The point was inserted, ID:" + vPointInserted);

                    //set feedback for client
                    //Generate message
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_success;
                    vMessage = "The point was successuly added with the following paramters:"+Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "ID:" + vPointInserted.Id + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Point Name:" + vPointInserted.PointName + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Table Name:" + vPointInserted.TableName + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Database Name:" + vPointInserted.DataBaseName + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Tag 1:" + vPointInserted.Tag1 + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Tag 2:" + vPointInserted.Tag2 + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "Tag 3:" + vPointInserted.Tag3 + Environment.NewLine;
                    EV.AddToList(vMessage);
                    vMessage = "MUT ID:" + vPointInserted.MUT_Id + Environment.NewLine;
                    EV.AddToList(vMessage);

                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    


                    model.EtronixValidation = EV;

                    //2020-11-16 Update all users with this point
                    //var vPointId = new SqlParameter("@PointId", SqlDbType.Int).Value =  vPointInserted.Id;
                    var vPointId = new SqlParameter("@PointId", vPointInserted.Id);

                    vPointId.SqlDbType = SqlDbType.Int;

                    var vUpdatePointsAccess = _contextEntities.Database.SqlQuery<int>(
                        "UpdatePointsAccessNewPoint @PointId ",
                        vPointId
                    ).ToList();

                    if (vUpdatePointsAccess[0] == 0)
                    {
                        alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_success;
                        EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                        EV.AddToList("All users were updated with this point");
                        model.EtronixValidation = EV;
                        return View(model);
                    }
                    else
                    {
                        alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                        EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                        EV.AddToList("The UpdatePointsAccess procedure created an error");
                        model.EtronixValidation = EV;
                        return View(model);
                    }
                }
                

            }
            else
            {
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vMessage = "Provide valid parameters";

                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);

                model.EtronixValidation = EV;
            }
            }
            catch (Exception e)
            {
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(e.Message);
                model.EtronixValidation = EV;
                return View(model);
            }

        _end:
            
            
            return View(model);
        }

        public ActionResult IndexTableActionResult(
            string pointName, 
            string tableName, 
            string databaseName,
            string tag1,
            string tag2,
            string tag3,
            int ? mutID,
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
                tag2 , 
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

            return PartialView("_Points_IndexTable", vPoints);
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

        [HttpGet]
        public ActionResult EditPoint(string pointID)
        {
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;
            List<Points> vEditedPoint = null;
            List<PointsMU> vCombinedLists = null;
            


            try { 


            int vId = int.Parse(pointID);
            
            Entities db = new Entities();
            vEditedPoint = db.Points.Where(m => m.Id == vId).ToList();

            List<Procedures.MeasurementUnitTypes> vM = db.MeasurementUnitType.ToList();

            vCombinedLists = vEditedPoint.Join(vM, a => a.MUT_id, b => b.Id, (a, b) => new PointsMU
               {
                        Id = a.Id,
                        PointName = a.PointName,
                        TableName = a.TableName,
                        DatabaseName = a.DatabaseName,
                        Tag1 = a.Tag1,
                        Tag2 = a.Tag2,
                        Tag3 = a.Tag3,
                        MUT_id = a.MUT_id,
                        Type = b.Type,
                        EtronixValidation = EV
                    
                    
               }).ToList();    

            int z = 0;

                
             


            }
            catch (Exception exception)
            {
                //Console.WriteLine(exception.Message);
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vMessage = exception.Message + Environment.NewLine;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);

            }

            PointsMU vPointsMu = vCombinedLists[0];

            return View(vPointsMu);
             
            //return Content("rrenio");
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
                tag2 , 
                tag3, 
                mutID, 
                sortColumn, 
                sortOrder, 
                v_currentpage, 
                ppageSize);


            return PartialView("_Points_IndexTable", vPoints);



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


            return PartialView("_Points_IndexTable", vPoints);



        }

        //delete User
        [HttpGet]
        public ActionResult DeletePoint(int pointID)
        {
            Procedures.ResultDeletePoint modelPoint = new Procedures.ResultDeletePoint();
            Entities _contextEntities = new Entities();
            EtronixValidation EV = new EtronixValidation();

            int alert = -1;
            string vMessage = null;

            try
            {
                //throw new Exception("Controlled Exception");
                if (pointID != null)
                {

                    var vPointID = new SqlParameter("@pointID", pointID);
                    var vPointIDResult = _contextEntities.Database
                        .SqlQuery<Procedures.ResultDeletePoint>("DeletePoint @pointID", vPointID).ToList();



                    //alert success
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_success;
                    vMessage = "Point ID:" + pointID.ToString() + " was successfully deleted" + Environment.NewLine;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    modelPoint.EtronixValidation = EV;
                    //when you delete a point the access information of this point must be deleted as well
                    EV.AddToList("The access information of this point was deleted for all users");
                }
            }
            catch (Exception exception)
            {
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vMessage = exception.Message;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);
                modelPoint.EtronixValidation = EV;
            }



            return PartialView("_Point_DeletePoint", modelPoint);
        }


        [HttpPost]
        public ActionResult EditPoint(PointsMU model)
        {
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;

            Entities _contextEntities = new Entities();
            try
            {
                if (ModelState.IsValid)
                {


                    var vPointId = new SqlParameter("@pointID", model.Id);
                    var vPointName = new SqlParameter("@PointName", model.PointName);
                    var vTableName = new SqlParameter("@TableName", model.TableName);
                    var vDatabaseName = new SqlParameter("@DatabaseName", model.DatabaseName);
                    var vTag1 = new SqlParameter("@Tag1", model.Tag1);
                    var vTag2 = new SqlParameter("@Tag2", model.Tag2);
                    var vTag3 = new SqlParameter("@Tag3", model.Tag3);
                    var vMUT_id = new SqlParameter("@MUT_id", model.MUT_id);


                    var vPointIDResult = _contextEntities.Database.SqlQuery<Procedures.ResultUpdatePoint>(
                        "UpdatePoint @PointId , @PointName , @TableName , @DatabaseName , @Tag1  , @Tag2  , @Tag3  , @MUT_id ", 
                         vPointId,
                         vPointName,
                         vTableName,
                         vDatabaseName,
                         vTag1,
                         vTag2,
                         vTag3,
                         vMUT_id
                         ).ToList();

                    alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_success;
                    vMessage = "The details' point were updated";

                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    model.EtronixValidation = EV;
                }
                else
                {
                    List<string> mE = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                    alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    foreach (var msg in mE)
                    {
                        EV.AddToList(msg);
                    }
                    model.EtronixValidation = EV;
                    return View(model);
                }
            }
            catch (Exception exception)
            {
                alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vMessage = exception.Message;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);
                model.EtronixValidation = EV;
            }

            return View(model);
        }

    }
}