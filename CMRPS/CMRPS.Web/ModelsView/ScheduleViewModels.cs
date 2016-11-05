using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMRPS.Web.Models;

namespace CMRPS.Web.ModelsView
{
    public class DetailsScheduleViewModel
    {
        public ScheduledModel Schedule { get; set; }
        public List<ComputerModel> Individual { get; set; }
        public ColorModel Color { get; set; }
        public LocationModel Location { get; set; }
        public ComputerTypeModel ComputerType { get; set; }
    }
}
