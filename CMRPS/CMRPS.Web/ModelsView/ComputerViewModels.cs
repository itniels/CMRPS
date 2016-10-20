using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMRPS.Web.Models;

namespace CMRPS.Web.ModelsView
{
    public class IndexComputerViewModels
    {
        public List<ComputerModel> Computers { get; set; }
        public List<ComputerTypeModel> ComputerTypes { get; set; }
        public List<ColorModel> ComputerColors { get; set; }
    }
}