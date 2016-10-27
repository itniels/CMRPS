using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMRPS.Web.Controllers
{
    public class ToolsController : Controller
    {
        /// <summary>
        /// GET | Network tools page.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult NetworkTools()
        {
            return View();
        }
    }
}