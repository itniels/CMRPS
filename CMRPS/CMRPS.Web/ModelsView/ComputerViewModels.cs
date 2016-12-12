using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        
        // Type
        [Required]
        [Display(Name = "Type:")]
        public string SelectedType { get; set; }
        // Color
        [Required]
        [Display(Name = "Color:")]
        public string SelectedColor { get; set; }
        // Location
        [Required]
        [Display(Name = "Location:")]
        public string SelectedLocation { get; set; }
        
    }
}