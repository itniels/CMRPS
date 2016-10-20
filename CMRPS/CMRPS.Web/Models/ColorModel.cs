using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    public class ColorModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Label Color")]
        public string ColorLabel { get; set; }

        [Display(Name = "Label Text")]
        public string ColorText { get; set; }

        public virtual List<ComputerModel> Computers { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}