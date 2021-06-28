using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAppEtronix20180824.App_Start.Other;

namespace WebAppEtronix20180824.Models.Entity
{
    public class Procedures
    {

        public class ResultMainMenu
        {
            public string Id { get; set; }
            public int MainMenuIndx { get; set; }
            public string MainMenu { get; set; }


        }

        public class ResultSubMenu
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Controller { get; set; }
            public string Action { get; set; }
            public int MainMenuIndx { get; set; }
            public int SubMenuIndx { get; set; }
            public string MainMenu { get; set; }
            public string SubMenu { get; set; }


        }

        public class ResultDeleteUser
        {
            public string userId { get; set; }
            public EtronixValidation EtronixValidation { get; set; }
        }

        public class ResultGetAllRoles
        {
            public int Id { get; set; }
            public string MainMenu { get; set; }
            public int MainMenuPosition { get; set; }
            public string SubMenu { get; set; }
            public int SubMenuPosition { get; set; }
            public string Controller { get; set; }
            public string Action { get; set; }
            public bool Access { get; set; }


        }

        public class ResultGetUserRoles
        {
            public int Id { get; set; }
            public int RoleId { get; set; }
            public string UserId { get; set; }

        }

        public class ResultGetUsersDetails
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public EtronixValidation EtronixValidation { get; set; }
        }

        public class MeasurementUnitTypes
        {
            public int ? Id { get; set; }
            public string Type { get; set; }
            
            
        }

        public class ResultDeleteMeasurementUnitTypes
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public EtronixValidation EtronixValidation { get; set; }

        }

        public class ResultInsertPoint
        {
            
            public int Id { get; set; }
            public string PointName { get; set; }
            public string TableName { get; set; }
            public string DataBaseName { get; set; }
            public string Tag1 { get; set; }
            public string Tag2 { get; set; }
            public string Tag3 { get; set; }
            public int MUT_Id { get; set; }


        }

        public class ResultDeletePoint
        {

            public int Id { get; set; }
            public string PointName { get; set; }
            public string TableName { get; set; }
            public string DataBaseName { get; set; }
            public string Tag1 { get; set; }
            public string Tag2 { get; set; }
            public string Tag3 { get; set; }
            public int MUT_Id { get; set; }
            public EtronixValidation EtronixValidation { get; set; }


        }

        public class ResultUpdatePoint
        {

            public int Id { get; set; }
            public string PointName { get; set; }
            public string TableName { get; set; }
            public string DataBaseName { get; set; }
            public string Tag1 { get; set; }
            public string Tag2 { get; set; }
            public string Tag3 { get; set; }
            public int MUT_Id { get; set; }


        }

        public class ResultAccess
        {
            public int Id { get; set; }
            public string ASPNetUserId { get; set; }
            public int PointId { get; set; }
            public bool Access { get; set; }


        }


    }
}
