using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMRPS.Web.Controllers
{
    public class StatusController : Controller
    {
        // GET: Status
        public ActionResult Overview()
        {
            return View();
        }

        public ActionResult ListView()
        {
            return View();
        }
    }
}