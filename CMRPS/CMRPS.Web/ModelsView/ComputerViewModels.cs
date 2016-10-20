using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;

namespace CMRPS.Web.ModelsView
{
    public class IndexComputerViewModels
    {
        public List<ComputerModel> Computers { get; set; }
        public List<ComputerTypeModel> ComputerTypes { get; set; }
        public List<ColorModel> ComputerColors { get; set; }
    }

    public class CreateComputerViewModel
    {
        // Computer Fields
        public ComputerModel Computer { get; set; }
        // Color
        public SelectList Colors { get; set; }
        // Location
        public SelectList Locations { get; set; }
        // Info
        public InfoModel Info { get; set; }
    }
}