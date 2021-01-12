using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebAppEtronix20180824.Models.Entity;
using WebAppEtronix20180824.Models.DTO;

namespace WebAppEtronix20180824.Controllers
{
    public class MainMenuController : Controller
    {
        private Entities _contextEntities;
        
        // GET: MainMenu
        public PartialViewResult GetMenu()
        {
            
            _contextEntities = new Entities();

            //User null - not logged or ID has a value - logged
            var vUserId = User.Identity.GetUserId();
            //Take MainMenu
            //Entity Framework DB Model
            //I have change schema and after on 12 of July 2019
            //20190712 the mode was not imported correctly 
            //the procedure GetMainMenu() correctly was defined to return value int instead of list, an error appeared
            //RK had to change from DB to CF from database
            //var vMainMenu = _contextEntities.GetMainMenu(vUserId).ToList();

            //Check if user is logged in
            if (vUserId != null)
            {
                //EF Code First
                //check all MainMenu elements for the logged in user
                var clientIdParameter = new SqlParameter("@UserID", vUserId);
                var vMainMenu = _contextEntities.Database
                    .SqlQuery<Procedures.ResultMainMenu>("GetMainMenu @UserId", clientIdParameter).ToList();

                Menu vMenu = new Menu();
                vMenu.MainMenu = new List<MainMenu>();

                foreach (var velement in vMainMenu)
                {
                    vMenu.MainMenu.Add(new MainMenu()
                    {
                        Indx = velement.MainMenuIndx,
                        Name = velement.MainMenu
                    });
                }


                //Bind all SubMenu elements to its parent (MainMenu element).

                foreach (var variable in vMenu.MainMenu)
                {

                    //EF DB first from database
                    // procedure GetSubMenu() was not importet correctly on 12 of July 2019 
                    //RK had to change to CF
                    //var vSubMenu = _contextEntities.GetSubMenu(vUserId,variable.Indx).ToList();

                    //EF code first from DB
                    var clientIdParameter2 = new SqlParameter("@UserID", vUserId);
                    var clientIdParameter21 = new SqlParameter("@MainMenuIndx", variable.Indx);
                    var vSubMenu = _contextEntities.Database.SqlQuery<Procedures.ResultSubMenu>(
                        "GetSubMenu @UserID,@MainMenuIndx",
                        clientIdParameter2, clientIdParameter21).ToList();

                    variable.SubMenu = new List<SubMenu>();
                    foreach (var varSubMenu in vSubMenu)
                    {
                        variable.SubMenu.Add(new SubMenu()
                        {
                            Indx = varSubMenu.SubMenuIndx,
                            Action = varSubMenu.Action,
                            Controller = varSubMenu.Controller,
                            Name = varSubMenu.SubMenu
                        });
                    }



                }
                return PartialView("_MainMenu2", vMenu.MainMenu);
            }
            else
            {

                return PartialView("_MainMenu2", null);
            }
            
            
        }
    }
}