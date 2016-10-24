using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;
using Hangfire;

namespace CMRPS.Web.Controllers
{
    public class JobsController : Controller
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private RecurringJobManager manager = new RecurringJobManager();
        public static bool Ping()
        {
            List<ComputerModel> computers = db.Computers.ToList();
            foreach (ComputerModel computer in computers)
            {
                BackgroundJob.Enqueue(() => ActionController.Ping(computer));
            }
            return false;
        }
        
    }
}