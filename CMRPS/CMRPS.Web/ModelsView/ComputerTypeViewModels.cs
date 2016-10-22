using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CMRPS.Web.Models;

namespace CMRPS.Web.ModelsView
{
    public class IndexComputerTypeViewModel
    {

    }

    public class CreateComputerTypeViewModel
    {
        public ComputerTypeModel ComputerType { get; set; }

        [Display(Name = "Image")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase UploadedFile { get; set; }
    }
}