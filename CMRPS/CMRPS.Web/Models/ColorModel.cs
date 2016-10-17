using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    public class ColorModel
    {
        public int Id { get; set; }

        public string Color { get; set; }

        public virtual List<ComputerModel> Computers { get; set; }
    }
}