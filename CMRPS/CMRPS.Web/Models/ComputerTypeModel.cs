﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    public class ComputerTypeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImagePath { get; set; }

        public virtual List<ComputerModel> Computers { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}