using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMRPS.Web.Models;

namespace CMRPS.Web.ModelsView
{
    public class CreateScheduleViewModel
    {
        public ScheduledModel Schedule { get; set; }
        public List<ComputerModel> Computers { get; set; }


    }
}