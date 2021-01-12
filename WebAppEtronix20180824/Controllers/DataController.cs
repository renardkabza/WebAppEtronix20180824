using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using PagedList;
using WebAppEtronix20180824.App_Start.Other;
using WebAppEtronix20180824.Models;
using WebAppEtronix20180824.Models.Entity;

namespace WebAppEtronix20180824.Controllers
{
    public class DataController : Controller
    {
        // GET: Data
        [HttpGet]
        public ActionResult Index(MeasurementTypeViewModel pDataViewModel)
        {

            if (ModelState.IsValid)
            {
                return View(pDataViewModel);    
            }
            else if (pDataViewModel.EtronixValidation != null)
            {
                return View(pDataViewModel);    
            }
            else
            {
                return View();
            }
        }

        //post
        [HttpPost]
        public ActionResult AddType(MeasurementTypeViewModel  pDataViewModel)
        {
            Entities _contextEntities;
            MeasurementTypeViewModel measurementTypeViewModel = new MeasurementTypeViewModel();
            measurementTypeViewModel.Id = pDataViewModel.Id;
            measurementTypeViewModel.Type = pDataViewModel.Type ;
            
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;
            
            if (ModelState["Type"].Errors.Count==0)
            {

                try
                {
                    //get details of the changing person
                    _contextEntities = new Entities();
                    var vMeasurementType = new SqlParameter("@Type", pDataViewModel.Type);
                    Procedures.MeasurementUnitTypes vMeasurementUnitTypes= _contextEntities.Database.SqlQuery<Procedures.MeasurementUnitTypes>(
                            "InsertMeasurementType " + "@Type ",
                            vMeasurementType).SingleOrDefault();

                    //2 commands clear the field of Unit_type
                    measurementTypeViewModel.Type = null;
                    ModelState.Clear();

                }
                catch (Exception exception)
                {
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    vMessage = exception.Message;
                    EV.ValidationCode=EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    measurementTypeViewModel.EtronixValidation = EV;
                    
                    goto End;
                }
                
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_success;
                vMessage = "The measurement type was successfully added";
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);
                measurementTypeViewModel.EtronixValidation = EV;
            }
            else
                {
                    List<string> mE = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    foreach (var msg in mE)
                    {
                        EV.AddToList(msg);
                    }
                    measurementTypeViewModel.EtronixValidation = EV;
                
                }


            //return RedirectToAction("Index", new RouteValueDictionary(new {Type ="haha"}));
            End:
            return View("Index", measurementTypeViewModel);

        }

        //Get: Return a tablet as a partial view
        public ActionResult IndexTableActionResult(string unit_type, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {
            var vMeasurementType = GetMeasurementType(unit_type, sortColumn, sortOrder, currentpage, ppageSize);


            return PartialView("_Data_IndexTable", vMeasurementType);
        }


        private IPagedList<Procedures.MeasurementUnitTypes> GetMeasurementType(string unit_type, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
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


            string vsortColumn = String.IsNullOrEmpty(sortColumn) ? "Unit Type" : sortColumn;
            IPagedList<Procedures.MeasurementUnitTypes> vMeasurementType = null;
            List<Procedures.MeasurementUnitTypes> vMasterMeasurementType = null;
            ViewBag.SortColumn = vsortColumn;


            ViewBag.CurrentPage = pageIndex;

            var flag = 1;
        takedata:
            switch (vsortColumn)
            {
                case "Unit Type":
                    if (sortOrder == "Desc")
                    {
                        //employees = db.Employees.Where(m=>m.Name.Contains("10")).OrderByDescending(m => m.Name).ToPagedList(pageIndex, pageSize);
                        //or
                        //employees = db.Employees.OrderByDescending(m => m.Name).ToPagedList(pageIndex, pageSize);
                        //or some exmple from the internet
                        /*_db.EMPLOYEEs.Where(x => x.EMAIL == givenInfo || x.USER_NAME == givenInfo)
                        .Select(x => new { x.EMAIL, x.ID }); */
                        vMasterMeasurementType = db.MeasurementUnitType.OrderByDescending(m => m.Type).ToList();

                    }
                    else
                    {
                        vMasterMeasurementType = db.MeasurementUnitType.OrderBy(m => m.Type).ToList();
                    }

                    break;
                
                default: //by  Name and ASC
                    vMeasurementType = db.MeasurementUnitType.OrderBy(m => m.Type).ToPagedList(pageIndex, pageSize);
                    break;
            }

            if (unit_type != "")
            {
                vMasterMeasurementType = vMasterMeasurementType.Where(m => m.Type.Contains(unit_type)).ToList();
            }
            

            vMeasurementType = vMasterMeasurementType.ToPagedList(pageIndex, pageSize);


            int rowsquantity = vMeasurementType.Count;


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
            return vMeasurementType;
        }

        //Goto Previous Page
        public ActionResult NextPageActionResult(string unit_type, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {

            int? v_currentpage = currentpage;
            v_currentpage++;
            var vMeasurementTypes = GetMeasurementType(unit_type, sortColumn, sortOrder, v_currentpage, ppageSize);


            return PartialView("_Data_IndexTable", vMeasurementTypes);



        }

        //Goto Previous Page
        public ActionResult PreviousPageActionResult(string unit_type, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {

            int? v_currentpage = currentpage;
            v_currentpage--;
            var vMeasurementTypes = GetMeasurementType(unit_type, sortColumn, sortOrder, v_currentpage, ppageSize);


            return PartialView("_Data_IndexTable", vMeasurementTypes);



        }

        //delete User
        [HttpGet]
        public ActionResult DeleteMeasurementUnitType(string measurementUnitType)
        {
            Procedures.ResultDeleteMeasurementUnitTypes modelMeasurementUnitTypes = new Procedures.ResultDeleteMeasurementUnitTypes();
            Entities _contextEntities = new Entities();
            EtronixValidation EV = new EtronixValidation();

            int alert = -1;
            string vMessage = null;

            try
            {
                //throw new Exception("Controlled Exception");
                if (measurementUnitType != null)
                {

                    var measurementUnitTypeID = new SqlParameter("@MUID", measurementUnitType);
                    var vMUT = _contextEntities.Database
                        .SqlQuery<Procedures.ResultDeleteMeasurementUnitTypes>("DeleteMeasurementUnitType @MUID", measurementUnitTypeID).ToList();

                    

                    //alert success
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_success;
                    vMessage = "Measurement Unit Type:" + measurementUnitType + " was successfully deleted" + Environment.NewLine;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    modelMeasurementUnitTypes.EtronixValidation = EV;
                }
            }
            catch (Exception exception)
            {
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vMessage = exception.Message;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);
                modelMeasurementUnitTypes.EtronixValidation = EV;    
            }
        


            return PartialView("_Data_DeleteMeasurementUnitType", modelMeasurementUnitTypes);
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult EditMeasurementType(string MUID)
        {

            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;
            Entities _contextEntities;

            UpdateMeasurementTypeViewModel vUpdateMeasurementTypeViewModel = new UpdateMeasurementTypeViewModel();



            try
            {
                _contextEntities = new Entities();
                var vParameter = new SqlParameter("@MUID", MUID);
                Procedures.MeasurementUnitTypes vMeasurementType =
                    _contextEntities.Database.SqlQuery<Procedures.MeasurementUnitTypes>(
                        "GetMeasurementType " + "@MUID ",
                        vParameter).SingleOrDefault();

                vUpdateMeasurementTypeViewModel.Id = vMeasurementType.Id.ToString();
                vUpdateMeasurementTypeViewModel.Type = vMeasurementType.Type;
                vUpdateMeasurementTypeViewModel.NewType = "";
                vUpdateMeasurementTypeViewModel.EtronixValidation = EV;
                
            }
            catch (Exception exception)
            {
                //Console.WriteLine(exception.Message);
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vMessage = exception.Message + Environment.NewLine;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);
                vUpdateMeasurementTypeViewModel.EtronixValidation = EV;

            }


            return View(vUpdateMeasurementTypeViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditMeasurementType(UpdateMeasurementTypeViewModel model)
        {
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;

            Entities _contextEntities = new Entities();

            //check if all fields are correctly filled with data - data validation
            if (ModelState.IsValid)
            {

                //UpdateMeasurementTypeViewModel vUpdateMeasurementTypeViewModel = new UpdateMeasurementTypeViewModel();
                
                try
                {
                    _contextEntities = new Entities();
                    var vUserParameter1 = new SqlParameter("@MUID", model.Id);
                    var vUserParameter2 = new SqlParameter("@MUType", model.NewType);
                    var vResult = _contextEntities.Database.SqlQuery<List<string>>(
                        "UpdateMeasurementType " +
                        "@MUID, " +
                        "@MUType",
                        vUserParameter1,
                        vUserParameter2).SingleOrDefault();

                    alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_success;
                    vMessage = "The Measurement Unit was updated";
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    model.EtronixValidation = EV;
                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception.Message);
                    alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    vMessage = exception.Message + Environment.NewLine;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    model.EtronixValidation = EV;

                }
            }

            else
            {
                List<string> mE = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                foreach (var msg in mE)
                {
                    EV.AddToList(msg);
                }
                model.EtronixValidation = EV;
                return View(model);
            }


            
            return View(model);
        }

    }
}