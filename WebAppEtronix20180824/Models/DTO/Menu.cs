using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppEtronix20180824.Models.DTO
{
    public class Menu
    {
        public List<MainMenu> MainMenu { get; set; }
    }

    public class MainMenu
    {
        public int ? Indx { get; set; }
        public string Name { get; set; }
        public List<SubMenu> SubMenu { get; set; }
    }

    public class SubMenu
    {
        public int ? Indx { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Name { get; set; }
    }
}