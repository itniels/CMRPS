using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMRPS.Web.ModelsView
{
    public class ImportViewModel
    {
        [Display(Name = "File:")]
        public HttpPostedFileBase File { get; set; }
    }
}